using Location.WCFServiceReferences.LocationServices;
using System;

public interface ICommunicationClient {
    void GetTags(Action<Tag[]> callback);

    void GetTag(int id, Action<Tag> callback);
    void GetPersonTree(Action<AreaNode> callback);

    void GetDepartmentTree(Action<Department> callback);


    void GetTopoTree(Action<PhysicalTopology> callback);

    void GetAreaStatistics(int id, Action<AreaStatistics> callback);

    void GetPointsByPid(int areaId, Action<AreaPoints[]> callback);
}
