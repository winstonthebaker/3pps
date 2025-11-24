using Godot;
using System;
using Utilities;

public partial class GroundEvaluator : ShapeCast3D
{
	private const float COYOTE_TIME = 0.2f; //seconds
	private const float MAX_GROUND_ANGLE = 45f; //degrees

	private struct CollisionInfo
	{
		public Vector3 Point;
		public Vector3 Normal;
	}

	private CircularBuffer<Tuple<ulong, CollisionInfo>> _groundCollisionBuffer = new CircularBuffer<Tuple<ulong, CollisionInfo>>(10);

	private void GetGroundingInfo()
    {
        
            int collisionCount = GetCollisionCount();
            for (int i = 0; i < collisionCount; i++)
            {
                Vector3 point = GetCollisionPoint(i);
                Vector3 normal = GetCollisionNormal(i);

                CollisionInfo info = new CollisionInfo
                {
                    Point = point,
                    Normal = normal
                };

                _groundCollisionBuffer.Enqueue(new Tuple<ulong, CollisionInfo>(Time.GetTicksUsec(), info));
            }
            while (_groundCollisionBuffer.TryPeek(out var tuple))
            {
                ulong timeElapsed = Time.GetTicksUsec() - tuple.Item1;
                if (TunaMath.USecToSec(timeElapsed) > COYOTE_TIME)
                {
                    _groundCollisionBuffer.Dequeue();
                }
                else
                {
                    break;
                }
            }
        
    }

    public bool IsGrounded()
    {
        foreach (var tuple in _groundCollisionBuffer)
        {
            Vector3 normal = tuple.Item2.Normal;
            float angle = Mathf.RadToDeg(Mathf.Acos(normal.Dot(Vector3.Up)));
            if (angle <= MAX_GROUND_ANGLE)
            {
                return true;
            }
        }
        return false;
    }

	public void Update()
	{
		GetGroundingInfo();
	}
	

}
