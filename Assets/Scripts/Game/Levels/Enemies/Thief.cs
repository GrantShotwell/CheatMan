using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace Game.Levels.Enemies
{
    public sealed class Thief : Enemy
    {

        protected override void Start()
        {
            base.Start();
            SetState(IdleState);
        }

        private async UniTask IdleState(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (SeenByPlayer)
                {
                    SetState(AttackState);
                }
                await UniTask.NextFrame(cancellationToken);
            }
        }

        private async UniTask AttackState(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await UniTask.WaitForSeconds(weaponSpeed);
                if (!SeenByPlayer)
                {
                    SetState(IdleState);
                    continue;
                }
                await AttackAsync(cancellationToken);
            }
        }

        private async UniTask AttackAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var pos = transform.localPosition;
                pos.x -= movementSpeed * Time.deltaTime;
                transform.localPosition = pos;
                await UniTask.NextFrame(PlayerLoopTiming.Update);
            }
        }
    }
}
