using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using MotusPhysics.Core;
using MotusPhysics.Core.Physics;
using MotusPhysics.Core.Physics.Colliders;
using MotusPhysics.Core.Physics.Data;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using Vector = MotusPhysics.Core.Utility.Vector;

namespace Main;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private float circleX = 400;
    private float circleY = 225;
    
    private float radius = 50;
    private float velocityX = 150f;

    private DispatcherTimer timer;
    private RigidBody circleBody;
    
    private Player player1;
    private Player player2;
    
    private Random random = new Random();
    
    public MainWindow()
    {
        InitializeComponent();
        Motus.Initialize();
        
        timer = new DispatcherTimer();
        timer.Interval = System.TimeSpan.FromMilliseconds(16); // ~60 FPS
        timer.Tick += Timer_Tick;
        timer.Start();
        
        circleBody = RigidBody.CreateRigidBody(Collider.CreateCircleCollider(radius), position: new Vector(circleX, circleY), restitution: 0.8d);
        circleBody.AddVelocity(new Vector(velocityX, 0));
        
        player1 = new Player(700f, 225f);
        player1.Body.OnCollisionEnterSubscribe(OnPlayer1CollisionEnter);
        this.KeyDown += player1.OnKeyDown;
        this.KeyUp += player1.OnKeyUp;
        player1.KeyBindings.Add("Up", Key.W);
        player1.KeyBindings.Add("Down", Key.S);
        Console.WriteLine(player1.Body.Id);
        
        player2 = new Player(100f, 225f);
        player2.Body.OnCollisionEnterSubscribe(OnPlayer2CollisionEnter);
        this.KeyDown += player2.OnKeyDown;
        this.KeyUp += player2.OnKeyUp;
        player2.KeyBindings.Add("Up", Key.Up);
        player2.KeyBindings.Add("Down", Key.Down);
        Console.WriteLine(player2.Body.Id);
    }

    private void OnPlayer2CollisionEnter(CollisionManifold manifold)
    {
        velocityX = -velocityX;
        Console.WriteLine(manifold.RigidBodyA.Id);
        manifold.RigidBodyA.SetVelocity(new Vector(velocityX, random.Next(-100, 100)));
    }

    private void OnPlayer1CollisionEnter(CollisionManifold manifold)
    {
        velocityX = -velocityX;
        Console.WriteLine(manifold.RigidBodyA.Id);
        manifold.RigidBodyA.SetVelocity(new Vector(velocityX, random.Next(-100, 100)));
    }

    private void Timer_Tick(object sender, System.EventArgs e)
    {
        if (circleBody.Position.x - radius < 0 || circleBody.Position.x + radius > (float)SkiaCanvas.ActualWidth)
        {
            velocityX = -velocityX;
            circleBody.SetVelocity(new Vector(velocityX, 0));
        }
        
        if (circleBody.Position.y - radius < 0 || circleBody.Position.y + radius > (float)SkiaCanvas.ActualHeight)
        {
            circleBody.SetVelocity(new Vector(circleBody.Velocity.x, -circleBody.Velocity.y));
        }
        
        if (circleBody.Position.x - radius < 0)
        {
            player2.Score++;
            circleBody.SetPosition(new Vector(circleX, circleY));
            circleBody.SetVelocity(new Vector(-velocityX, 0));
        }
        else if (circleBody.Position.x + radius > (float)SkiaCanvas.ActualWidth)
        {
            player1.Score++;
            circleBody.SetPosition(new Vector(circleX, circleY));
            circleBody.SetVelocity(new Vector(velocityX, 0));
        }

        SkiaCanvas.InvalidateVisual(); // Triggers PaintSurface
    }

    private void SkiaCanvas_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
    {
        var canvas = e.Surface.Canvas;
        canvas.Clear(SKColors.White);

        using var paint = new SKPaint
        {
            Color = SKColors.DeepSkyBlue,
            IsAntialias = true
        };
        
        using var TextPaint = new SKPaint
        {
            Color = SKColors.Black,
            IsAntialias = true
        };
        
        using var playerPaint = new SKPaint
        {
            Color = SKColors.Red,
            IsAntialias = true
        };
        
        using var player2Paint = new SKPaint
        {
            Color = SKColors.Green,
            IsAntialias = true
        };
        
        var player1Position = player1.GetPosition();
        var player2Position = player2.GetPosition();

        canvas.DrawCircle((float)circleBody.Position.x, (float)circleBody.Position.y, radius, paint);
        canvas.DrawRect((float)player1Position.x, (float)player1Position.y, player1.Width, player1.Height, playerPaint);
        canvas.DrawRect((float)player2Position.x, (float)player2Position.y, player2.Width, player2.Height, player2Paint);
        canvas.DrawText($"Player 1: {player1.Score}", 10, 20, TextPaint);
        canvas.DrawText($"Player 2: {player2.Score}", (float)SkiaCanvas.ActualWidth - 100, 20, TextPaint);
    }
}