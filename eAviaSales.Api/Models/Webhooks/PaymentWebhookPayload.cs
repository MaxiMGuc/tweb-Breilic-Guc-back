using System.ComponentModel.DataAnnotations;

namespace eAviaSales.Api.Models.Webhooks;

/// <summary>Minimal PSP-shaped payload for local testing.</summary>
public sealed class PaymentWebhookPayload
{
    [Required]
    public required string PaymentId { get; init; }

    [Required]
    public required string Status { get; init; }
}

