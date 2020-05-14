using System;

public abstract class Actor
{
    public abstract void Action(Actor target, Action action);
    public abstract void Actioned(HostActor actor, Action action);
    public abstract void Actioned(ClientActor actor, Action action);
    public abstract void Actioned(EnemyActor actor, Action action);
}

public class HostActor : Actor
{
    public override void Action(Actor target, Action action) => target.Actioned(this, action);
    public override void Actioned(HostActor actor, Action action) { action(); }
    public override void Actioned(ClientActor actor, Action action) { action(); }
    public override void Actioned(EnemyActor actor, Action action) { action(); }
}

public class ClientActor : Actor
{
    public override void Action(Actor target, Action action) => target.Actioned(this, action);
    public override void Actioned(HostActor actor, Action action) { }
    public override void Actioned(ClientActor actor, Action action) { }
    public override void Actioned(EnemyActor actor, Action action) { }
}

public class EnemyActor : Actor
{
    public override void Action(Actor target, Action action) => target.Actioned(this, action);
    public override void Actioned(HostActor actor, Action action) { action(); }
    public override void Actioned(ClientActor actor, Action action) { }
    public override void Actioned(EnemyActor actor, Action action) { }
}
