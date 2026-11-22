namespace Echo
{
    public class PlantEchoSpawner : EchoSpawnerBase<PlantEcho>
	{
        protected override void Awake()
        {
            base.Awake();
            SetValues(transform, generalPrefabConfigurations.PlantEcho, _objectAmount);
        }
    }
}