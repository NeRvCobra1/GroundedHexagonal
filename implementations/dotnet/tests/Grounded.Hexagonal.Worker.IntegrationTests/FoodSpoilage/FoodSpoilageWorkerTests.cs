using Grounded.Hexagonal.Adapters.Inbound.Worker.FoodSpoilage;
using Grounded.Hexagonal.Adapters.Outbound.Persistence.InMemory;
using Grounded.Hexagonal.Application.Ports.Inbound;
using Grounded.Hexagonal.Application.Ports.Outbound;
using Grounded.Hexagonal.Application.UseCases.ProcessFoodSpoilage;
using Grounded.Hexagonal.Domain.Food;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Grounded.Hexagonal.Worker.IntegrationTests.FoodSpoilage;

public sealed class FoodSpoilageWorkerTests
{
    [Fact]
    public async Task RunOnceAsync_Should_Execute_UseCase_And_Spoil_Due_Food()
    {
        var now = new DateTimeOffset(
            2026, 9, 20, 12, 0, 0, TimeSpan.Zero);

        var dueFood = new Domain.Food.Food(
            FoodId.New(),
            now);

        var freshFood = new Domain.Food.Food(
            FoodId.New(),
            now.AddMinutes(5));

        var repository = new InMemoryFoodRepository(
            [dueFood, freshFood]);

        var handler = new ProcessFoodSpoilageHandler(
            repository,
            new FixedClock(now));

        var worker = new FoodSpoilageWorker(
            handler,
            Options.Create(new FoodSpoilageWorkerOptions
            {
                Interval = TimeSpan.FromMinutes(1),
                RunImmediately = true
            }),
            NullLogger<FoodSpoilageWorker>.Instance);

        var result = await worker.RunOnceAsync();

        Assert.Equal(2, result.EvaluatedCount);
        Assert.Equal(1, result.SpoiledCount);
        Assert.True(dueFood.IsSpoiled);
        Assert.False(freshFood.IsSpoiled);
    }


    [Fact]
    public async Task BackgroundService_Should_Invoke_Inbound_Port_When_Started()
    {
        var useCase = new RecordingUseCase();

        var worker = new FoodSpoilageWorker(
            useCase,
            Options.Create(new FoodSpoilageWorkerOptions
            {
                Interval = TimeSpan.FromMinutes(10),
                RunImmediately = true
            }),
            NullLogger<FoodSpoilageWorker>.Instance);

        await worker.StartAsync(CancellationToken.None);

        await useCase.FirstInvocation.Task.WaitAsync(
            TimeSpan.FromSeconds(2));

        await worker.StopAsync(CancellationToken.None);

        Assert.Equal(1, useCase.Calls);
    }

    private sealed class FixedClock : IClock
    {
        public FixedClock(DateTimeOffset utcNow)
        {
            UtcNow = utcNow;
        }

        public DateTimeOffset UtcNow { get; }
    }

    private sealed class RecordingUseCase : IProcessFoodSpoilageUseCase
    {
        public int Calls { get; private set; }

        public TaskCompletionSource<bool> FirstInvocation { get; } =
            new(TaskCreationOptions.RunContinuationsAsynchronously);

        public Task<ProcessFoodSpoilageResult> ExecuteAsync(
            ProcessFoodSpoilageCommand command,
            CancellationToken cancellationToken = default)
        {
            Calls++;
            FirstInvocation.TrySetResult(true);

            return Task.FromResult(
                new ProcessFoodSpoilageResult(
                    EvaluatedCount: 0,
                    SpoiledCount: 0));
        }
    }
}
