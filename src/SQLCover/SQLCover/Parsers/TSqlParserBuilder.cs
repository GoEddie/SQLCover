using System;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using SQLCover.Objects;

namespace SQLCover.Parsers
{
    internal static class TSqlParserBuilder
    {
        public static TSqlParser BuildNew(SqlServerVersion version, bool quoted)
        {
            switch (version)
            {
                case SqlServerVersion.Sql90:
                    return new TSql90Parser(quoted);

                case SqlServerVersion.Sql100:
                    return new TSql100Parser(quoted);

                case SqlServerVersion.Sql110:
                    return new TSql110Parser(quoted);

                case SqlServerVersion.Sql120:
                    return new TSql120Parser(quoted);

                case SqlServerVersion.Sql130:
                    return new TSql130Parser(quoted);

                case SqlServerVersion.Sql140:
                    return new TSql130Parser(quoted);

                case SqlServerVersion.Sql150:
                    return new TSql130Parser(quoted);

                default:
                    throw new ArgumentOutOfRangeException(nameof(version), version, null);
            }
        }
    }
}