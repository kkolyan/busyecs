using Leopotam.EcsLite;

namespace Kk.BusyEcs
{
    public readonly struct Entity
    {
        private readonly EcsWorld _world;
        private readonly int _id;

        public Entity(EcsWorld world, int id)
        {
            _world = world;
            _id = id;
        }

        public EntityRef AsRef()
        {
            return new EntityRef(_world.PackEntityWithWorld(_id));
        }

        public ref T Add<T>() where T : struct
        {
            return ref _world.GetPool<T>().Add(_id);
        }

        public void DelEntity()
        {
            _world.DelEntity(_id);
        }

        public void Del<T>() where T : struct
        {
            _world.GetPool<T>().Del(_id);
        }

        public ref T Get<T>() where T : struct
        {
            return ref _world.GetPool<T>().Get(_id);
        }

        public bool Has<T>() where T : struct
        {
            return _world.GetPool<T>().Has(_id);
        }
    }
}