﻿using System.Transactions;

namespace chldr_shared.tests
{
    public class TranslationServiceTests
    {
        [Fact]
        public async Task TranslationService_CRUD_Success()
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
            //    // Arrange
            //    var dataProvider = TestDataFactory.CreateSqlDataProvider();
            //    var entryService = TestDataFactory.CreateEntryService();

            //    var unitOfWork = dataProvider.CreateUnitOfWork();
            //    var actingUserId = unitOfWork.Users.GetRandomsAsync(10).Result.First(u => u.Status == chldr_data.Enums.UserStatus.Active).UserId;

            //    var source = (await unitOfWork.Sources.GetRandomsAsync(1)).First();

            //    // Act
            //    var entry = TestDataFactory.CreateRandomEntryDto(actingUserId, source.SourceId);
            //    await entryService.AddAsync(entry, actingUserId);

            //    var translation = await translationService.GetAsync(entry.TranslationsDtos[0].TranslationId);
            //    await translationService.RemoveAsync(translation.TranslationId, actingUserId);

            //    // Assert
            //    await Assert.ThrowsAsync<InvalidArgumentsException>(async () => await translationService.GetAsync(entry.TranslationsDtos[0].TranslationId));
            }
        }
    }
}
