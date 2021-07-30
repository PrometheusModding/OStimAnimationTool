using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OStimAnimationTool.Core
{
    internal class MyReferenceResolver : ReferenceResolver
    {
        private uint _referenceCount;
        private readonly Dictionary<string, object> _referenceIdToObjectMap = new ();
        private readonly Dictionary<object, string> _objectToReferenceIdMap = new (ReferenceEqualityComparer.Instance);

        public override void AddReference(string referenceId, object value)
        {
            if (!_referenceIdToObjectMap.TryAdd(referenceId, value))
            {
                throw new JsonException();
            }
        }

        public override string GetReference(object value, out bool alreadyExists)
        {
            if (_objectToReferenceIdMap.TryGetValue(value, out var referenceId))
            {
                alreadyExists = true;
            }
            else
            {
                _referenceCount++;
                referenceId = _referenceCount.ToString();
                _objectToReferenceIdMap.Add(value, referenceId);
                alreadyExists = false;
            }

            return referenceId;
        }

        public override object ResolveReference(string referenceId)
        {
            if (!_referenceIdToObjectMap.TryGetValue(referenceId, out var value))
            {
                throw new JsonException();
            }

            return value;
        }
    }

    internal class MyReferenceHandler : ReferenceHandler
    {
        public MyReferenceHandler() => Reset();
        private ReferenceResolver? _rootedResolver;
        public override ReferenceResolver CreateResolver() => _rootedResolver!;
        public void Reset() => _rootedResolver = new MyReferenceResolver();
    }
}
