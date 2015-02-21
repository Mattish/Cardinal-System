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
            var newIdRequest = new HeathCliffNewIdRequest();
            DoAsserts(newIdRequest);
        }

        [Test]
        public void Deserialize_CreatesCorrectType_HeathCliffNewIdResponse()
        {
            var newIdRequest = new HeathCliffNewIdResponse();

            DoAsserts(newIdRequest);
        }

        [Test]
        public void Deserialize_CreatesCorrectType_HeathCliffOrderConnect()
        {
            var newIdRequest = new HeathCliffOrderConnect();

            DoAsserts(newIdRequest);
        }

        [Test]
        public void Deserialize_CreatesCorrectType_HeathCliffOrderDisconnect()
        {
            var newIdRequest = new HeathCliffOrderDisconnect();

            DoAsserts(newIdRequest);
        }

        private void DoAsserts(Message original)
        {
            var newIdRequestDto = original.ToDto();
            var dtoAsString = JsonConvert.SerializeObject(newIdRequestDto);
            MessageDto deserializeMessageDto = null;
            Assert.DoesNotThrow(() =>
            {
                deserializeMessageDto = JsonConvert.DeserializeObject<MessageDto>(dtoAsString, _messageDtoConverter);
            });

            Assert.That(deserializeMessageDto, Is.Not.Null);
            Assert.That(deserializeMessageDto.GetType(), Is.EqualTo(newIdRequestDto.GetType()));
        }
    }
}
