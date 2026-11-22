namespace Sounds
{
    public class SoundManager
    {
        public SFXAudioHandler SfxAudioHandler { get; private set; } = new SFXAudioHandler();
        public AmbienceAudioHandler AmbienceAudioHandler { get; private set; } = new AmbienceAudioHandler();
        public InitializerAudioHandler FactorySoundAudioHandler { get; private set; } = new InitializerAudioHandler();

    }
}

