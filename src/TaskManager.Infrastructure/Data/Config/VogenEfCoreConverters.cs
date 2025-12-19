using TaskManager.Core.BoardAggregate;
using TaskManager.Core.UserAggregate;
using Vogen;

namespace TaskManager.Infrastructure.Data.Config;

[EfCoreConverter<BoardId>]
[EfCoreConverter<BoardName>]
[EfCoreConverter<ColumnId>]
[EfCoreConverter<ColumnName>]
[EfCoreConverter<CardId>]
[EfCoreConverter<CardTitle>]
[EfCoreConverter<CardDescription>]
[EfCoreConverter<ColumnOrder>]
[EfCoreConverter<CardOrder>]
[EfCoreConverter<UserId>]
[EfCoreConverter<UserName>]
[EfCoreConverter<UserEmail>]
[EfCoreConverter<UserPassword>]
[EfCoreConverter<BoardRole>]
[EfCoreConverter<RefreshToken>]
internal partial class VogenEfCoreConverters;
