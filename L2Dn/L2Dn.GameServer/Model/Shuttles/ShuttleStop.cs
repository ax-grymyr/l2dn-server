﻿namespace L2Dn.GameServer.Model.Shuttles;

public class ShuttleStop
{
    private readonly int _id;
    private bool _isOpen = true;
    private readonly List<Location> _dimensions = new(3);
    private DateTime _lastDoorStatusChanges = DateTime.UtcNow;

    public ShuttleStop(int id)
    {
        _id = id;
    }

    public int getId()
    {
        return _id;
    }

    public bool isDoorOpen()
    {
        return _isOpen;
    }

    public void addDimension(Location loc)
    {
        _dimensions.Add(loc);
    }

    public List<Location> getDimensions()
    {
        return _dimensions;
    }

    public void openDoor()
    {
        if (_isOpen)
        {
            return;
        }

        _isOpen = true;
        _lastDoorStatusChanges = DateTime.UtcNow;
    }

    public void closeDoor()
    {
        if (!_isOpen)
        {
            return;
        }

        _isOpen = false;
        _lastDoorStatusChanges = DateTime.UtcNow;
    }

    public bool hasDoorChanged()
    {
        return (DateTime.UtcNow - _lastDoorStatusChanges) <= TimeSpan.FromSeconds(1);
    }
}