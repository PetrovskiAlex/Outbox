using System;
using Microsoft.Extensions.Options;

namespace Outbox
{
    public class OutboxOptions
    {
        public int Retries { get; set; } = 3;
        public TimeSpan ProcessorDelay { get; set; } = TimeSpan.FromSeconds(1);
        public int MessagesToProcess { get; set; } = 100;
    }

    public class OutboxOptionsValidator : IValidateOptions<OutboxOptions>
    {
        public ValidateOptionsResult Validate(string name, OutboxOptions options)
        {
            if (options == null)
            {
                return ValidateOptionsResult.Fail("Configuration object is null.");
            }

            if (options.Retries <= 0 || options.Retries > 100)
            {
                return ValidateOptionsResult.Fail("Retries should be greater than 0 and less than 100");
            }

            if (options.ProcessorDelay < TimeSpan.FromSeconds(1) || options.ProcessorDelay > TimeSpan.FromHours(1))
            {
                return ValidateOptionsResult.Fail("ProcessorDelay should be greater or equal 1 second and less than 1 hour");
            }

            if (options.MessagesToProcess <= 0 || options.MessagesToProcess > 10_000)
            {
                return ValidateOptionsResult.Fail("MessagesToProcess should be greater than 0 and less than 10_000");
            }

            return ValidateOptionsResult.Success;
        }
    }
}