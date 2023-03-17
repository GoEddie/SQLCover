using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.XPath;
using SQLCover.Objects;

namespace SQLCover.Parsers
{
    public class EventsParser
    {
        private readonly List<string> _xmlEvents;
        private XDocument _doc;

        private int _stringNumber;

        public EventsParser(List<string> xmlEvents)
        {
            _xmlEvents = xmlEvents;
            if (_xmlEvents == null || _xmlEvents.Count == 0)
            {
                _xmlEvents = new List<string>();
                return;
            }
            
            _doc = XDocument.Parse(xmlEvents[_stringNumber++]);
        }

        public CoveredStatement GetNextStatement()
        {
            if (_stringNumber > _xmlEvents.Count || _xmlEvents.Count == 0)
                return null;


            var statement = new CoveredStatement();

            statement.Offset = GetOffset();
            statement.OffsetEnd = GetOffsetEnd();
            statement.ObjectId = GetIntValue("object_id");
            statement.RowCount = GetRowCount();

            if (_stringNumber < _xmlEvents.Count)
                _doc = XDocument.Parse(_xmlEvents[_stringNumber++]);
            else
                _stringNumber++;
            
            return statement;
        }

        private int GetOffset()
        {
            var value = GetStringValue("offset");
            if (value == null)
            {
                value = Get2008StyleString("offsetStart");
            }

            return int.Parse(value);
        }

        private int GetOffsetEnd()
        {
            var value = GetStringValue("offset_end");
            if (value == null)
            {
                value = Get2008StyleString("offsetEnd");
            }

            var offset = int.Parse(value);
            return offset;
        }

        private int GetRowCount()
        {
            var value = GetStringValue("row_count");
            if (value == null)
            {
                value = Get2008StyleString("rowCount");
            }
            return int.Parse(value);
        }

        private string Get2008StyleString(string name)
        {
            var node = _doc.XPathSelectElement("//action[@name='tsql_stack']/value");
            var newDocument = XDocument.Parse(string.Format("<Root>{0}</Root>", node.Value));

            var frame = newDocument.Element("Root").FirstNode;

            if (frame == null)
            {
                return null;
            }

            var element = frame as XElement;
            return element.Attribute(name).Value;
        }

        private int GetIntValue(string name)
        {
            var value = GetStringValue(name);
            return int.Parse(value);
        }


        private string GetStringValue(string name)
        {
            var node = _doc.XPathSelectElement(string.Format("//data[@name='{0}']/value", name));

            return node?.Value;
        }
    }
}