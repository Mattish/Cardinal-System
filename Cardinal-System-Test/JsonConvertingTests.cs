using Cardinal_System_Shared;
using Cardinal_System_Shared.Component;
using Cardinal_System_Shared.Dto;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Cardinal_System_Test
{
    [TestFixture]
    public class JsonConvertingTests
    {
        private MessageDtoConverter _messageDtoConverter;

        [SetUp]
        public void SetUp()
        {
            _messageDtoConverter = new MessageDtoConverter();
        }

        [Test]
        public void Deserialize_CreatesCorrectType_HeathCliffNewIdRequest()
        {
            var newIdRequest = new HeathCliffNewIdRequest(ComponentType.Circuit, "SomeIpAddress", 1234);
            DoAsserts(newIdRequest);
        }

        [Test]
        public void Deserialize_CreatesCorrectType_HeathCliffNewIdResponse()
        {
            var newIdRequest = new HeathCliffNewIdResponse(1234);

            DoAsserts(newIdRequest);
        }

        [Test]
        public void Deserialize_CreatesCorrectType_HeathCliffOrderConnect()
        {
            var newIdRequest = new HeathCliffOrderConnect("SomeString", 1234, 1, ComponentType.Circuit);
            DoAsserts(newIdRequest);
        }

        [Test]
        public void Deserialize_CreatesCorrectType_HeathCliffOrderDisconnect()
        {
            var newIdRequest = new HeathCliffOrderDisconnect();

            DoAsserts(newIdRequest);
        }

        [Test]
        public void Deserialize_CreatesCorrectType_Heartbeat()
        {
            var newIdRequest = new Heartbeat();
            newIdRequest.SourceComponent = 1234;

            var result = DoAsserts(newIdRequest);
            Assert.That(result.SourceComponent, Is.EqualTo(newIdRequest.SourceComponent));
            var resultMessage = result.TranslateFromDto();
            Assert.That(resultMessage.SourceComponent, Is.EqualTo(newIdRequest.SourceComponent));
        }

        [Test]
        public void Deserialize_CreatesCorrectType_ConnectionToComponent()
        {
            var newIdRequest = new ConnectionToComponent(123);

            DoAsserts(newIdRequest);
        }

        [Test]
        public void Deserialize_CreatesCorrectType_DisconnectionToComponent()
        {
            var newIdRequest = new DisconnectionFromComponent(123);

            DoAsserts(newIdRequest);
        }

        [Test]
        public void Deserialize_CreatesCorrectType_ComponentInformationBroadcast()
        {
            var newIdRequest = new ComponentInformationBroadcast(123, ComponentType.Circuit);

            DoAsserts(newIdRequest);
        }

        private MessageDto DoAsserts(Message original)
        {
            var newIdRequestDto = original.ToDto();
            var dtoAsString = JsonConvert.SerializeObject(newIdRequestDto);
            MessageDto deserializeMessageDto = null;
            Assert.DoesNotThrow(
                () =>
                {
                    deserializeMessageDto = JsonConvert.DeserializeObject<MessageDto>(dtoAsString, _messageDtoConverter);
                });

            Assert.That(deserializeMessageDto, Is.Not.Null);
            Assert.That(deserializeMessageDto.GetType(), Is.EqualTo(newIdRequestDto.GetType()));
            return deserializeMessageDto;
        }
    }
}