using System;
using UnityEngine;

namespace Kk.BusyEcs.Test
{
    // define phase attributes you need. Start and Update are enough for simple case.

    [EcsPhase]
    [AttributeUsage(AttributeTargets.Method)]
    public class Start : Attribute { }

    [EcsPhase]
    [AttributeUsage(AttributeTargets.Method)]
    public class Update : Attribute { }
    
    // define BusyECS project-wide configuration.
    internal static class BusyEcsConfig
    {
#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
#endif
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void Configure()
        {
            // supply reference to the C# assembly where our ECS components and systems resides.
            // in case if you use asmdefs and your components and systems are distributed among them,
            // then you have to to specify all of them: by typeofing some class from each, for exampleю
            // (note that this example assumes that BusyEcsConfig is defined outside of Editor directory)
            BusyEcs.SetUserAssemblies(typeof(BusyEcsConfig).Assembly);
        }
    }
    
    // configure main loop. in Unity main loop is hidden by MonoBehavior scripts abstraction, so use them in case of Unity.
    
    public class ExampleUnityStartup : MonoBehaviour
    {
        private IEcsContainer _ecs;

        private void Start()
        {
            _ecs = new EcsContainerBuilder(EcsMetadata.ScanProject())
                // register any services for DI
                .AddInjectable(new SomeService())
                .Create();

            // force framework to invoke all system methods attributed with [Start]
            _ecs.Execute<Start>();
        }

        private void Update()
        {
            // force framework to invoke all system methods attributed with [Update]
            _ecs.Execute<Update>();
        }
    }

    // EcsWorld attribute says that this component relates to "events" world, which is used during EcsWorld resolution at the entity creation stage.
    // In this example very simple approach is used when all long-living entities associated with default world and short-living (one-frame) has
    // dedicated "events" world. That is not ideal solution, but reasonable compromise between boilerplate and efficiency.
    [EcsWorld("events")]
    public struct Damage
    {
        public float amount;

        // use EntityRef to keep safe references to entities in components and anywhere else outside of local scope.
        public EntityRef target;
    }

    [EcsWorld("events")]
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
        public float value;
    }

    public struct Armor
    {
        public float value;
    }

    public struct Regen
    {
        public float perSecond;
    }

    // attribute class with [EcsSystem] to make it visible for scanner
    [EcsSystem]
    public class ExampleSystem
    {
        [Inject] private SomeService _someService = default;

        // IEnv is built-in service. no need to declare it with "Injectable()".
        [Inject] private IEnv _env = default;

        // because Update phase triggered once per frame
        [Update]
        public void GlobalCallback() { }

        [Start]
        public void CreateCharacter()
        {
            // create entity
            // note that you are obliged to specify at least one component. that's needed for EcsWorld resolution. in this particular case, default world is used, because no initial component declares its world.
            Entity character = _env.NewEntity(
                new Position { value = Vector3.zero },
                new Velocity { value = Vector3.forward }
            );

            // this new entity allocated within "events" world, because Damage declares so.
            _env.NewEntity(new Damage { amount = 42, target = character.AsRef() });

            // "events" world is used too, because Damage declares "events" archetype and TimeToLive doesn't object against
            _env.NewEntity(new Damage { amount = 36, target = character.AsRef() }, new TimeToLive { seconds = 2.7f });
        }

        // you may also specify any number of arguments after phase. they are components to match entities against.
        // this method will be invoked every frame for each entity with Damage component, as this method is invoked in each iteration of vanilla EcsFilter
        [Update]
        public void LogDamage(Damage damage)
        {
            Debug.Log($"damage: {damage.amount}");
        }

        // use "ref" keyword if component modification is intended
        [Update]
        public void ApplyVelocity(Update update, Velocity velocity, ref Position position)
        {
            position.value += velocity.value * Time.deltaTime;
        }

        //  "Entity" (built-in type) parameter can be passed before components, if component access is not enough and you need to manipulate matched entity
        [Update]
        public void DoOperations(Update update, Entity entity, ref Velocity velocity, ref Position position)
        {
            if (velocity.value.magnitude > 299_792_458)
            {
                entity.Del<Position>();
            }

            // the same, but entity reference accessible as parameter to do contextual action
            if (entity.Has<Health>())
            {
                entity.Get<Health>().value -= 17;
            }
            else
            {
                entity.Add<Health>().value = 100;
            }

            if (entity.Get<Health>().value <= 0)
            {
                entity.DelEntity();
            }
        }

        // access another entity by reference
        [Update]
        public void AccessAnotherEntity1(Entity entity, Damage damage)
        {
            //  damage.target is EntityRef
            bool refValidAndCriteriaMatch = damage.target.Match((Entity target, ref Health health, ref Armor armor) =>
            {
                health.value -= Mathf.Max(0, damage.amount - armor.value);
            });

            entity.DelEntity();
        }

        // access another reference by reference
        [Update]
        public void AccessAnotherEntity2(Entity entity, Damage damage)
        {
            if (damage.target.Deref(out Entity target))
            {
                bool criteriaMatch = target.Match((ref Health health, ref Armor armor) =>
                {
                    health.value -= Mathf.Max(0, damage.amount - armor.value);
                });
            }

            entity.DelEntity();
        }

        // explicit querying of another entities by component set (what is EcsFilter used for in vanilla)
        [Update]
        public void SubQuery(Entity entity, Explosion explosion)
        {
            Vector3 explosionPos = entity.Get<Position>().value;

            // a bit naive way to detect neighbours, but just for Query demonstration
            // note that all worlds are queried under the hood
            _env.Query((Entity candidate, ref Position pos, ref Health __) =>
            {
                if ((pos.value - explosionPos).magnitude <= explosion.radius)
                {
                    _env.NewEntity(new Damage { target = candidate.AsRef(), amount = explosion.damage });
                }
            });
        }
    }
}