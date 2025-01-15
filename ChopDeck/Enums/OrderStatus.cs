namespace ChopDeck.Enums
{
    public enum OrderStatus
    {
        PendingPayment,
        PaymentConfirmed,
        PendingRestaurantConfirmation,
        AcceptedByRestaurant,
        DeclinedByRestaurant,
        OrderPrepared,
        AssignedToDriver,
        DriverAtRestaurant,
        OrderInTransit,
        DriverAtAddress,
        OrderDelivered
    }

}