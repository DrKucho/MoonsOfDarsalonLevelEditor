namespace SWizRuntime
{
	public interface ISpriteCollectionForceBuild
	{
		bool UsesSpriteCollection(SWizSpriteCollectionData spriteCollection);
		void ForceBuild();
	}
}
