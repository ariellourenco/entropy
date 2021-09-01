using Xunit;

namespace System.Text.Json
{
    public static class SnakeCaseTests
    {
        private static readonly JsonSerializerOptions _options = new();

        static SnakeCaseTests() =>
            _options.PropertyNamingPolicy = new JsonSnakeCaseNamingPolicy();

        [Fact]
        public static void CanSerializeObjectToJson()
        {
            // Arrange
            var booking = new
            {
                BookingDate = new DateTimeOffset(2019, 06, 23, 22, 00, 00, TimeSpan.FromHours(1)),
                Id = "af43ea6f-b3ff-4640-9a9a-dbfc7544a4a4",
                Title = "Sample Booking",
                Premium = false,
                Price = new
                {
                    Value = 9.99M,
                    Currency = "GBP"
                },
                Member = new
                {
                    EmailAddress = "sample.member@somedomain.com",
                    FirstName = "William",
                    LastName = "McDowell",
                    Id = "7ce13464-a9df-4630-a50b-7fdd8a3661c4"
                }
            };

            // Act
            var json = JsonSerializer.Serialize(booking, _options);

            // Assert
            Assert.Contains("booking_date", json);
            Assert.Contains("first_name", json);
            Assert.Contains("last_name", json);
            Assert.Contains("email_address", json);
            Assert.Contains("price", json);
        }

        [Theory]
        [InlineData("URL", "url")]
        [InlineData("URLValue", "url_value")]
        [InlineData("ID", "id")]
        [InlineData("I", "i")]
        [InlineData("", "")]
        [InlineData("Person", "person")]
        [InlineData("iPhone", "i_phone")]
        [InlineData("IPhone", "i_phone")]
        [InlineData("I Phone", "i_phone")]
        [InlineData("I  Phone", "i_phone")]
        [InlineData(" IPhone", "_i_phone")]
        [InlineData(" IPhone ", "_i_phone_")]
        [InlineData("IsCIA", "is_cia")]
        [InlineData("VmQ", "vm_q")]
        [InlineData("Xml2Json", "xml_2_json")]
        [InlineData("SnAkEcAsE", "sn_ak_ec_as_e")]
        [InlineData("SnA__kEcAsE", "sn_a_k_ec_as_e")]
        [InlineData("SnA__ kEcAsE", "sn_a_k_ec_as_e")]
        [InlineData("already_snake_case_ ", "already_snake_case_")]
        [InlineData("IsJSONProperty", "is_json_property")]
        [InlineData("SHOUTING_CASE", "shouting_case")]
        [InlineData("Hi!! This is text. Time to test.", "hi_this_is_text_time_to_test_")]
        [InlineData("BUILDING", "building")]
        [InlineData("BUILDING Property", "building_property")]
        [InlineData("Building Property", "building_property")]
        [InlineData("BUILDING PROPERTY", "building_property")]
        public static void ToSnakeCaseTest(string input, string output) =>
            Assert.Equal(output, ConvertToSnakeCase(input));

        private static string ConvertToSnakeCase(string name)
        {
            var policy = new JsonSnakeCaseNamingPolicy();
            var value = policy.ConvertName(name);

            return value;
        }
    }
}
