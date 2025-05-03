using System.Windows.Input;
using MotusPhysics.Core.Physics;
using MotusPhysics.Core.Physics.Colliders;
using MotusPhysics.Core.Utility;

namespace Main;

public class Player
{
    public RigidBody Body { get; set; }
    public float Speed = 200f;
    public float Width = 50f;
    public float Height = 100f;
    public float StartPosX = 700f;
    public float StartPosY = 225f;
    public int Score = 0;
    public Dictionary<string, Key> KeyBindings = new();
    
    public Player(float startX, float startY)
    {
        StartPosX = startX;
        StartPosY = startY;
        Body = RigidBody.CreateRigidBody(Collider.CreateRectangleCollider(new Vector(Width, Height)), position: new Vector(StartPosX, StartPosY), restitution: 1d);
    }
    
    public void OnKeyUp(object sender, KeyEventArgs e)
    {
        Body.SetVelocity(new Vector(0, 0));
    }

    public void OnKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == KeyBindings["Up"])
        {
            Body.SetVelocity(new Vector(0, -Speed));
        }
        else if (e.Key == KeyBindings["Down"])
        {
            Body.SetVelocity(new Vector(0, Speed));
        }
    }

    public Vector GetPosition()
    {
        var position = new Vector(Body.Position.x - (Width / 2), Body.Position.y - (Height / 2));
        return position;
    }
}