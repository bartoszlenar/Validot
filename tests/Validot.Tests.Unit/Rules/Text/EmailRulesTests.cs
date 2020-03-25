namespace Validot.Tests.Unit.Rules.Text
{
    using Validot.Testing;
    using Validot.Translations;

    using Xunit;

    public class EmailRulesTests
    {
        [Theory]
        [InlineData(@"prettyandsimple@example.com", true)]
        [InlineData(@"very.common@example.com", true)]
        [InlineData(@"disposable.style.email.with+symbol@example.com", true)]
        [InlineData(@"other.email-with-dash@example.com", true)]
        [InlineData(@"fully-qualified-domain@example.com.", false)]
        [InlineData(@"user.name+tag+sorting@example.com", true)]
        [InlineData(@"x@example.com", true)]
        [InlineData(@"example-indeed@strange-example.com", true)]
        [InlineData(@"admin@mailserver1", false)]
        [InlineData(@"email@123.123.123.123", true)]
        [InlineData(@"email@[123.123.123.123]", true)]
        [InlineData(@"1234567890@example.com", true)]
        [InlineData(@"#!$%&'*+-/=?^_`{}|~@example.org", false)]
        [InlineData(@"""()<>[]:,;@\\\""!#$%&'-/=?^_`{}| ~.a""@example.org", true)]
        [InlineData(@"Abc.example.com", false)]
        [InlineData(@"A@b@c@example.com", false)]
        [InlineData(@"a""b(c)d,e:f;g<h>i[j\k]l@example.com", false)]
        [InlineData(@"just""not""right@example.com", false)]
        [InlineData(@"this is""not\allowed@example.com", false)]
        [InlineData(@"this\ still\""not\\allowed@example.com", false)]
        [InlineData(@"Duy", false)]
        [InlineData(@" email@example.com", false)]
        [InlineData(@"email@example.com ", false)]
        [InlineData(@"", false)]
        [InlineData(@"david.jones@proseware.com", true)]
        [InlineData(@"d.j@server1.proseware.com", true)]
        [InlineData(@"jones@ms1.proseware.com", true)]
        [InlineData(@"j.@server1.proseware.com", false)]
        [InlineData(@"j@proseware.com9", true)]
        [InlineData(@"js#internal@proseware.com", true)]
        [InlineData(@"j_9@[129.126.118.1]", true)]
        [InlineData(@"j..s@proseware.com", false)]
        [InlineData(@"js*@proseware.com", false)]
        [InlineData(@"js@proseware..com", false)]
        [InlineData(@"js@proseware.com9", true)]
        [InlineData(@"j.s@server1.proseware.com", true)]
        [InlineData(@"""j""s""@proseware.com", true)]
        [InlineData(@"js@contoso.中国", true)]
        public void Email_Should_CollectError(string model, bool expectedIsValid)
        {
            Tester.TestSingleRule(
                model,
                m => m.Email(),
                expectedIsValid,
                MessageKey.Texts.Email);
        }
    }
}
