using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Xml;
using webapi_saml_test_shared.Utilities;

namespace webapi_saml_test_shared.Models
{
    public class SAMLData
    {
        public Dictionary<string, string> SAMLAttributes;

        public UserData UserData = new UserData();

        public SAMLData(AssertionType assertion)
        {
            // Find the attribute statement within the assertion
            AttributeStatementType ast = null;
            foreach (StatementAbstractType sat in assertion.Items)
            {
                if (sat.GetType().Equals(typeof(AttributeStatementType)))
                {
                    ast = (AttributeStatementType)sat;
                }
            }

            if (assertion.Conditions.NotBeforeSpecified)
            {
                UserData.IssuedDate = DateTime.SpecifyKind(assertion.Conditions.NotBefore, DateTimeKind.Utc);
            }
            else
            {
                UserData.IssuedDate = DateTime.Now;
            }

            if (assertion.Conditions.NotOnOrAfterSpecified)
            {
                UserData.ExpirationDate = DateTime.SpecifyKind(assertion.Conditions.NotOnOrAfter, DateTimeKind.Utc);
            }
            else
            {
                UserData.ExpirationDate = DateTime.Now;
            }

            if (ast == null)
            {
                throw new ApplicationException("Invalid SAML Assertion: Missing Attribute Values");
            }

            SAMLAttributes = new Dictionary<string, string>();

            // Do what needs to be done to pull specific attributes out for sending on
            // For now assuming this is a simple list of string key and string values
            foreach (AttributeType at in ast.Items)
            {
                // We may not need to add every attribute here.
                SAMLAttributes.Add(at.Name, GetSAMLValue(at));

                switch (at.Name)
                {
                    case "UserId":
                        UserData.UserId = GetSAMLValue(at);
                        break;
                }
            }
        }

        private string GetSAMLValue(AttributeType at)
        {
            string response = string.Empty;

            object[] value = at.AttributeValue;
            XmlNode[] property = (XmlNode[])at.AttributeValue[0];
            if (property != null)
            {
                XmlNode textNode = property.FirstOrDefault(x => x.NodeType == XmlNodeType.Text);
                if (textNode != null)
                {
                    response = textNode.Value.ToString();
                }
            }

            return response;
        }
    }
}