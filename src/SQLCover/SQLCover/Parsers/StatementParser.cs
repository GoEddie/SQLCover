using System.Collections.Generic;
using System.IO;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using SQLCover.Objects;

namespace SQLCover.Parsers
{
    public class StatementParser
    {
        private readonly SqlServerVersion _version;

        public StatementParser(SqlServerVersion version)
        {
            _version = version;
        }

        public List<Statement> GetChildStatements(string script, bool quotedIdentifier)
        {
            var visitor = new StatementVisitor(script);
            var parser = TSqlParserBuilder.BuildNew(_version, quotedIdentifier);


            IList<ParseError> errors;
            var fragment = parser.Parse(new StringReader(script), out errors);
            if (fragment == null)
            {
                return null;
            }

            fragment.Accept(visitor);


            return visitor.Statements;
        }
    }

    public class StatementVisitor : TSqlFragmentVisitor
    {
        private readonly string _script;
        public readonly List<Statement> Statements = new List<Statement>();
        private bool _stopEnumerating = false;

        public StatementVisitor(string script)
        {
            _script = script;
        }

        public override void Visit(TSqlStatement statement)
        {
            if (_stopEnumerating)
                return;

            if (ShouldNotEnumerateChildren(statement))
            {
                Statements.Add(new Statement (_script.Substring(statement.StartOffset, statement.FragmentLength),  statement.StartOffset, statement.FragmentLength, false));
                _stopEnumerating = true;       //maybe ExplicitVisit would be simpler??
                return;
            }

            base.Visit(statement);

            if (!IsIgnoredType(statement))
            {
                Statements.Add(new Statement (_script.Substring(statement.StartOffset, statement.FragmentLength), statement.StartOffset, statement.FragmentLength, CanBeCovered(statement)));
            }

            if (statement is IfStatement)
            {
                var ifStatement = (IfStatement) statement;

                var branches = new List<Branch>(2) {
                    new Branch(
                        _script.Substring(ifStatement.ThenStatement.StartOffset, ifStatement.ThenStatement.FragmentLength),
                        ifStatement.ThenStatement.StartOffset,
                        ifStatement.ThenStatement.FragmentLength
                    )
                };

                if (ifStatement.ElseStatement != null)
                {
                    branches.Add(
                        new Branch(
                            _script.Substring(ifStatement.ElseStatement.StartOffset, ifStatement.ElseStatement.FragmentLength),
                            ifStatement.ElseStatement.StartOffset,
                            ifStatement.ElseStatement.FragmentLength
                        )
                    );
                }

                var offset = statement.StartOffset;
                var length = ifStatement.Predicate.StartOffset + ifStatement.Predicate.FragmentLength - statement.StartOffset;
                Statements.Add(new Statement(_script.Substring(offset, length), offset, length, CanBeCovered(statement), branches));
            }

            if (statement is WhileStatement)
            {
                var whileStatement = (WhileStatement) statement;

                var branches = new [] {
                    new Branch(
                        _script.Substring(whileStatement.Statement.StartOffset, whileStatement.Statement.FragmentLength),
                        whileStatement.Statement.StartOffset,
                        whileStatement.Statement.FragmentLength
                    )
                };

                var offset = statement.StartOffset;
                var length = whileStatement.Predicate.StartOffset + whileStatement.Predicate.FragmentLength - statement.StartOffset;
                Statements.Add(new Statement (_script.Substring(offset, length), offset, length, CanBeCovered(statement), branches));
            }
        }

        private bool IsIgnoredType(TSqlStatement statement)
        {
            if (statement is BeginEndBlockStatement)
                return true;

            if (statement is WhileStatement)
                return true;

            if (statement is IfStatement)
                return true;

            //next todo - in extended events i think if are handled differently - do we need to search for the predicate and have that a s asepararte thinggy???

            return !CanBeCovered(statement); //TODO: This all needs tidying up, don't think i need two things to do the same thing???
        }


        private bool ShouldNotEnumerateChildren(TSqlStatement statement)
        {
            if (statement is CreateViewStatement)
                return true;

            return false;
        }
        private bool CanBeCovered(TSqlStatement statement)
        {
            if (statement is BeginEndBlockStatement)
                return false;

            if (statement is TryCatchStatement)
                return false;
                        
            if (statement is CreateProcedureStatement)
                return false;

            if (statement is CreateFunctionStatement)
                return false;

            if (statement is CreateTriggerStatement)
                return false;

            if (statement is DeclareVariableStatement)
                return false;

            if (statement is DeclareTableVariableStatement)
                return false;

            if (statement is LabelStatement)
                return false;

            return true;
        }
    }
}