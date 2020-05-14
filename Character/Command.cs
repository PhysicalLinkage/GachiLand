using UnityEngine;

public class Command
{
    public Vector3 Point { get; set; }
    public GameObject Target { get; set; }

    public Command()
    {
        Point = Vector3.zero;
        Target = null;
    }

    public Command(Vector3 point)
    {
        Point = point;
        Target = null;
    }

    public Command(GameObject target)
    {
        Point = Vector3.zero;
        Target = target;
    }

    public Command(Vector3 point, GameObject target)
    {
        Point = point;
        Target = target;
    }
}
