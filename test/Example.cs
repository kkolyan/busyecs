using UnityEngine;

namespace Kk.BusyEcs
{
    // define phase-state structs you need. Start and Update are enough for simple case.
    // that's just a structs with some attribute. 

    [EcsPhase]
    public struct StartPhase { }

    // they can carry values, if you like.
    [EcsPhase]
    public struct UpdatePhase
    {
        public float dt;
    }

    // configure main loop. in Unity main loop is hidden by MonoBehavior scripts abstraction, so use them in case of Unity.

    public class ExampleUnityStartup : MonoBehaviour
    {
        private EcsContainer _ecs;

        private void Start()
        {
            _ecs = new EcsContainer.Builder()
                // register any services for DI
                .Injectable(new SomeService())
                .End();

            // force framework to invoke all systems that handle StartPhase
            _ecs.Execute(new StartPhase());
        }

        private void Update()
        {
            // force framework to invoke all systems that handle UpdatePhase
            _ecs.Execute(new UpdatePhase { dt = Time.deltaTime });
        }
    }

    // This attribute says that this component relates to "one-frames" archetype, which is used during archetype resolution at the entity creation stage.
    // This example shows very simple approach when all long-living entities associated with default archetype and short-living (one-frame) has
    // dedicated "events" archetype. That is not ideal solution, but reasonable compromise between boilerplate and efficiency.
    [EcsArchetype("events")]
    public struct Damage
    {
        public float damage;

        // use EntityRef to keep safe references to entities in components and anywhere else outside of local scope.
        public EntityRef target;
    }

    [EcsArchetype("events")]
    public struct Explosion
    {
        public float radius;
        public float damage;
    }

    public struct Position
    {
        public Vector3 value;
    }

    public struct Velocity
    {
        public Vector3 value;
    }

    public struct TimeToLive
    {
        public float seconds;
    }

    public struct Health
    {
        public float health;
    }

    [EcsSystemClass]
    public class ExampleSystemClass
    {
        [Inject] private SomeService _someService = default;

        // IEnv is built-in service. no need to declare it with "Injectable()".
        [Inject] private IEnv _env = default;

        // first parameter of ecs-system method should be a phase descriptor. following method invoked once per frame, because this phase triggered once per frame
        [EcsSystem]
        public void GlobalCallback(UpdatePhase _) { }

        [EcsSystem]
        public void CreateCharacter(StartPhase _)
        {
            // create entity
            // note that you are obliged to specify at least one component. that's needed for archetype resolution. in this particular case, default archetype is used, because no initial component declares its archetype.
            Entity character = _env.NewEntity(
                new Position { value = Vector3.zero },
                new Velocity { value = Vector3.forward }
            );

            // this new entity allocated within "events" archetype, because Damage declares so.
            _env.NewEntity(new Damage { damage = 42, target = character.AsRef() });

            // "events" archetype is used too, because Damage declares "events" archetype and TimeToLive doesn't object against
            _env.NewEntity(new Damage { damage = 36, target = character.AsRef() }, new TimeToLive { seconds = 2.7f });
        }

        // you may also specify any number of arguments after phase. they are components to match entities against.
        // this method will be invoked every frame for each entity with Damage component, as this method is invoked in each iteration of vanilla EcsFilter
        [EcsSystem]
        public void LogDamage(UpdatePhase _, Damage damage)
        {
            Debug.Log($"damage: {damage.damage}");
        }

        // use "ref" keyword if component modification is intended
        [EcsSystem]
        public void ApplyVelocity(UpdatePhase update, Velocity velocity, ref Position position)
        {
            position.value += velocity.value * update.dt;
        }

        //  "Entity" (built-in type) parameter can be passed before components, if component access is not enough and you need to manipulate matched entity
        [EcsSystem]
        public void DoOperations(UpdatePhase _, Entity entity, ref Velocity velocity, ref Position position)
        {
            if (velocity.value.magnitude > 299_792_458)
            {
                entity.Del<Position>();
            }

            // the same, but entity reference accessible as parameter to do contextual action
            if (entity.Has<Health>())
            {
                entity.Get<Health>().health -= 17;
            }
            else
            {
                entity.Add<Health>().health = 100;
            }

            if (entity.Get<Health>().health <= 0)
            {
                entity.DelEntity();
            }
        }

        // access another entity by reference
        [EcsSystem]
        public void ApplyDamage(UpdatePhase _, Entity entity, Damage damage)
        {
            if (damage.target.TryDeref(out Entity target))
            {
                target.Get<Health>().health -= damage.damage;
            }

            entity.DelEntity();
        }

        // explicit querying of another entities by component set (what is EcsFilter used for in vanilla)
        [EcsSystem]
        public void Explode(UpdatePhase _, Entity entity, Explosion explosion)
        {
            Vector3 explosionPos = entity.Get<Position>().value;

            // a bit naive way to detect neighbours, but just for Query demonstration
            _env.Query((Entity candidate, ref Position pos, ref Health __) =>
            {
                if ((pos.value - explosionPos).magnitude <= explosion.radius)
                {
                    _env.NewEntity(new Damage { target = candidate.AsRef(), damage = explosion.damage });
                }
            });
        }
    }
}