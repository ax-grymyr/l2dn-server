using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public struct ExServerPrimitivePacket: IOutgoingPacket
{
    private readonly string _name;
    private readonly int _x;
    private readonly int _y;
    private readonly int _z;
    private readonly List<Point> _points;
    private readonly List<Line> _lines;

    public ExServerPrimitivePacket(string name, int x, int y, int z)
    {
        _name = name;
        _x = x;
        _y = y;
        _z = z;
        _points = new();
        _lines = new();
    }

    public string Name => _name;
    
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(ServerPacketCode.EXTENDED, ServerExPacketCode.EX_SERVER_PRIMITIVE);
        writer.WriteString(_name);
        writer.WriteInt32(_x);
        writer.WriteInt32(_y);
        writer.WriteInt32(_z);
        writer.WriteInt32(65535); // has to do something with display range and angle
        writer.WriteInt32(65535); // has to do something with display range and angle
        writer.WriteInt32(_points.Count + _lines.Count);
        foreach (Point point in _points)
        {
            writer.WriteByte(1); // Its the type in this case Point
            writer.WriteString(point.getName());
            int color = point.getColor();
            writer.WriteInt32((color >> 16) & 0xFF); // R
            writer.WriteInt32((color >> 8) & 0xFF); // G
            writer.WriteInt32(color & 0xFF); // B
            writer.WriteInt32(point.isNameColored() ? 1 : 0);
            writer.WriteInt32(point.getX());
            writer.WriteInt32(point.getY());
            writer.WriteInt32(point.getZ());
        }

        foreach (Line line in _lines)
        {
            writer.WriteByte(2); // Its the type in this case Line
            writer.WriteString(line.getName());
            int color = line.getColor();
            writer.WriteInt32((color >> 16) & 0xFF); // R
            writer.WriteInt32((color >> 8) & 0xFF); // G
            writer.WriteInt32(color & 0xFF); // B
            writer.WriteInt32(line.isNameColored() ? 1 : 0);
            writer.WriteInt32(line.getX());
            writer.WriteInt32(line.getY());
            writer.WriteInt32(line.getZ());
            writer.WriteInt32(line.getX2());
            writer.WriteInt32(line.getY2());
            writer.WriteInt32(line.getZ2());
        }
    }

    public void addPoint(Color color, int x, int y, int z)
    {
        addPoint(string.Empty, color, false, x, y, z);
    }

    public void addPoint(string name, Color color, bool isNameColored, int x, int y, int z)
    {
        _points.Add(new Point(name, color.Value, isNameColored, x, y, z));
    }

    public void addLine(Color color, int x, int y, int z, int x2, int y2, int z2)
    {
        addLine(string.Empty, color, false, x, y, z, x2, y2, z2);
    }

    public void addLine(string name, Color color, bool isNameColored, int x, int y, int z, int x2, int y2, int z2)
    {
        _lines.Add(new Line(name, color.Value, isNameColored, x, y, z, x2, y2, z2));
    }

    private class Point
    {
        private readonly String _name;
        private readonly int _color;
        private readonly bool _isNameColored;
        private readonly int _x;
        private readonly int _y;
        private readonly int _z;

        public Point(String name, int color, bool isNameColored, int x, int y, int z)
        {
            _name = name;
            _color = color;
            _isNameColored = isNameColored;
            _x = x;
            _y = y;
            _z = z;
        }

        /**
         * @return the name
         */
        public String getName()
        {
            return _name;
        }

        /**
         * @return the color
         */
        public int getColor()
        {
            return _color;
        }

        /**
         * @return the isNameColored
         */
        public bool isNameColored()
        {
            return _isNameColored;
        }

        /**
         * @return the x
         */
        public int getX()
        {
            return _x;
        }

        /**
         * @return the y
         */
        public int getY()
        {
            return _y;
        }

        /**
         * @return the z
         */
        public int getZ()
        {
            return _z;
        }
    }

    private class Line: Point
    {
        private readonly int _x2;
        private readonly int _y2;
        private readonly int _z2;

        public Line(String name, int color, bool isNameColored, int x, int y, int z, int x2, int y2, int z2): base(name,
            color, isNameColored, x, y, z)
        {
            _x2 = x2;
            _y2 = y2;
            _z2 = z2;
        }

        /**
         * @return the x2
         */
        public int getX2()
        {
            return _x2;
        }

        /**
         * @return the y2
         */
        public int getY2()
        {
            return _y2;
        }

        /**
         * @return the z2
         */
        public int getZ2()
        {
            return _z2;
        }
    }
}