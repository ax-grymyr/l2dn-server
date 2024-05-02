namespace L2Dn.Geometry;

public interface ILocation: ILocation3D
{
    int Heading { get; }
}