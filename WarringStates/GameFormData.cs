using LocalUtilities.SimpleScript.Serialization;
using LocalUtilities.TypeGeneral;

namespace WarringStates;

internal class GameFormData : FormData
{
    public override string LocalName { get; set; } = nameof(GameForm);

    public override Size MinimumSize { get; set; } = new(200, 200);

    protected override void DeserializeFormData(SsDeserializer deserializer)
    {

    }

    protected override void SerializeFormData(SsSerializer serializer)
    {

    }
}
