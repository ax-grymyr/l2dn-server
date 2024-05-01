namespace L2Dn.Geometry;

public interface ILocationHeading: ILocation3D
{
    int Heading { get; }
}