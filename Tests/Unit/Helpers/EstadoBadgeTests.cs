using MultiservicioB.Helpers;

namespace MultiservicioB.Tests.Unit.Helpers;

public sealed class EstadoBadgeTests
{
    [Theory]
    [InlineData(" ACTIVO ", EstadoBadge.Activo)]
    [InlineData("Mantenimiento crítico", EstadoBadge.Peligro)]
    [InlineData("Pendiente de confirmación", EstadoBadge.Atencion)]
    [InlineData("En progreso", EstadoBadge.Informativo)]
    [InlineData("estado desconocido", EstadoBadge.Inactivo)]
    public void General_NormalizesAndMapsSpanishStates(string state, string expectedClass)
    {
        var badgeClass = EstadoBadge.General(state);

        Assert.Equal(expectedClass, badgeClass);
    }

    [Fact]
    public void OrdenServicio_UsesWarningForPendingOrders()
    {
        var badgeClass = EstadoBadge.OrdenServicio("Pendiente");

        Assert.Equal("idx-badge-warning", badgeClass);
    }

    [Fact]
    public void CuentaEmpleado_ReflectsRegistrationState()
    {
        Assert.Equal(EstadoBadge.Activo, EstadoBadge.CuentaEmpleado(registrada: true));
        Assert.Equal(EstadoBadge.Inactivo, EstadoBadge.CuentaEmpleado(registrada: false));
    }
}
