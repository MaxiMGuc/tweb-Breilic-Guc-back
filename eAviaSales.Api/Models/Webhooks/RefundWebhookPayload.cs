using System.ComponentModel.DataAnnotations;

namespace eAviaSales.Api.Models.Webhooks;

public sealed class RefundWebhookPayload
{
    [Required]
    public required string RefundId { get; init; }

    [Required]
    public required string Status { get; init; }
}

