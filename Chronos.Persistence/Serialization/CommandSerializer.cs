using System.IO;
using Chronos.Infrastructure.Interfaces;
using Chronos.Persistence.Types;

namespace Chronos.Persistence.Serialization
{
    public class CommandSerializer : ICommandSerializer
    {
        private readonly ISerializer _serializer;

        public CommandSerializer(ISerializer serializer)
        {
            _serializer = serializer;
        }

        public Command Serialize(ICommand command)
        {
            Command serialized;

            using (var writer = new StringWriter())
            {
                _serializer.Serialize(writer,command);

                serialized = new Command
                {
                    TimestampUtc = command.Timestamp.ToDateTimeUtc(),
                    Payload = writer.ToString()
                };
            }

            return serialized;
        }

        public ICommand Deserialize(Command command)
        {
            using (var reader = new StringReader(command.Payload))
            {
                var icommand = _serializer.Deserialize<ICommand>(reader);
                return icommand;
            }
        }
    }
}