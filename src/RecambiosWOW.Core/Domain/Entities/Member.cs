using RecambiosWOW.Core.Domain.Enums;
using RecambiosWOW.Core.Domain.ValueObjects;

namespace RecambiosWOW.Core.Domain.Entities;

public class Member
{
    public int Id { get; private set; }
    public MemberIdentity Identity { get; private set; }
    public MembershipType Type { get; private set; }
    public MembershipPlan Plan { get; private set; }
    private readonly List<RegisteredVehicle> _registeredVehicles;
    public IReadOnlyCollection<RegisteredVehicle> RegisteredVehicles => _registeredVehicles.AsReadOnly();
    private readonly List<VehicleModel> _vehiclesOfInterest;
    public IReadOnlyCollection<VehicleModel> VehiclesOfInterest => _vehiclesOfInterest.AsReadOnly();
    private readonly List<PartsWatchList> _watchLists;
    public IReadOnlyCollection<PartsWatchList> WatchLists => _watchLists.AsReadOnly();
    public ActivityHistory ActivityHistory { get; private set; }
    public AuditInfo AuditInfo { get; private set; }

    public Member(
        MemberIdentity identity,
        MembershipType type,
        MembershipPlan plan)
    {
        Identity = identity ?? throw new ArgumentNullException(nameof(identity));
        Type = type;
        Plan = plan ?? throw new ArgumentNullException(nameof(plan));
        _registeredVehicles = new List<RegisteredVehicle>();
        _vehiclesOfInterest = new List<VehicleModel>();
        _watchLists = new List<PartsWatchList>();
        ActivityHistory = new ActivityHistory();
        AuditInfo = new AuditInfo(DateTime.UtcNow);
    }

    public void UpdatePlan(MembershipPlan newPlan)
    {
        Plan = newPlan ?? throw new ArgumentNullException(nameof(newPlan));
        AuditInfo = AuditInfo.Update();
    }

    public void AddVehicleOfInterest(VehicleModel vehicle)
    {
        if (!_vehiclesOfInterest.Contains(vehicle))
        {
            _vehiclesOfInterest.Add(vehicle);
            ActivityHistory.AddActivity(ActivityType.VehicleInterestAdded, $"Added interest in {vehicle.Make} {vehicle.Model} {vehicle.Year}");
            AuditInfo = AuditInfo.Update();
        }
    }

    protected Member()
    {
        _registeredVehicles = new List<RegisteredVehicle>();
        _vehiclesOfInterest = new List<VehicleModel>();
        _watchLists = new List<PartsWatchList>();
    }
}