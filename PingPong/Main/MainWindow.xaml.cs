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

    private RigidBody player1;
    private float player1Speed = 100f;
    private float player1Width = 50f;
    private float player1Height = 100f;
    private float player1X = 50f;
    private float player1Y = 225f;
    private int player1Score = 0;
    
    private RigidBody player2;
    private float player2Speed = 100f;
    private float player2Width = 50f;
    private float player2Height = 100f;
    private float player2X = 700f;
    private float player2Y = 225f;
    private int player2Score = 0;
    
    private Random random = new Random();
    
    public MainWindow()
    {
        this.KeyDown += OnKeyDown;
        this.KeyUp += OnKeyUp;
        InitializeComponent();
        Motus.Initialize();
        
        timer = new DispatcherTimer();
        timer.Interval = System.TimeSpan.FromMilliseconds(16); // ~60 FPS
        timer.Tick += Timer_Tick;
        timer.Start();
        
        circleBody = RigidBody.CreateRigidBody(Collider.CreateCircleCollider(radius), position: new Vector(circleX, circleY), restitution: 0.8d);
        circleBody.AddVelocity(new Vector(velocityX, 0));
        
        player1 = RigidBody.CreateRigidBody(Collider.CreateRectangleCollider(new Vector(player1Width, player1Height)), position: new Vector(player1X, player1Y), restitution: 0.8d);
        player1.OnCollisionEnterSubscribe(OnPlayer1CollisionEnter);
        Console.WriteLine(player1.Id);
        
        player2 = RigidBody.CreateRigidBody(Collider.CreateRectangleCollider(new Vector(player2Width, player2Height)), position: new Vector(player2X, player2Y), restitution: 0.8d);
        player2.OnCollisionEnterSubscribe(OnPlayer2CollisionEnter);
        Console.WriteLine(player2.Id);
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

    private void OnKeyUp(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.W || e.Key == Key.S)
        {
            player1.SetVelocity(new Vector(0, 0));
        }
        
        if (e.Key == Key.Up || e.Key == Key.Down)
        {
            player2.SetVelocity(new Vector(0, 0));
        }
    }

    private void OnKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.W)
        {
            player1.SetVelocity(new Vector(0, -player1Speed));
        }
        else if (e.Key == Key.S)
        {
            player1.SetVelocity(new Vector(0, player1Speed));
        }
        
        if (e.Key == Key.Up)
        {
            player2.SetVelocity(new Vector(0, -player2Speed));
        }
        else if (e.Key == Key.Down)
        {
            player2.SetVelocity(new Vector(0, player2Speed));
        }
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
            player2Score++;
            circleBody.SetPosition(new Vector(circleX, circleY));
            circleBody.AddVelocity(new Vector(-velocityX, 0));
        }
        else if (circleBody.Position.x + radius > (float)SkiaCanvas.ActualWidth)
        {
            player1Score++;
            circleBody.SetPosition(new Vector(circleX, circleY));
            circleBody.AddVelocity(new Vector(velocityX, 0));
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

        canvas.DrawCircle((float)circleBody.Position.x, (float)circleBody.Position.y, radius, paint);
        canvas.DrawRect((float)player1.Position.x, (float)player1.Position.y, player1Width, player1Height, playerPaint);
        canvas.DrawRect((float)player2.Position.x, (float)player2.Position.y, player2Width, player2Height, player2Paint);
        canvas.DrawText($"Player 1: {player1Score}", 10, 20, TextPaint);
        canvas.DrawText($"Player 2: {player2Score}", (float)SkiaCanvas.ActualWidth - 100, 20, TextPaint);
    }
}