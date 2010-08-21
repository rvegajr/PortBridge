//---------------------------------------------------------------------------------
// Microsoft (R) .NET Services 
// 
// Copyright (c) Microsoft Corporation. All rights reserved.  
//
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES 
// OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE. 
//---------------------------------------------------------------------------------

namespace PortBridge
{
    using System;
    using System.Configuration;

    public class HostMappingCollection : ConfigurationElementCollection
    {
        public HostMappingCollection()
        {
        }

        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.AddRemoveClearMap;
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new HostMappingElement();
        }

        protected override Object GetElementKey(ConfigurationElement element)
        {
            return ((HostMappingElement)element).TargetHost;
        }

        public HostMappingElement this[int index]
        {
            get
            {
                return (HostMappingElement)BaseGet(index);
            }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        new public HostMappingElement this[string targetHost]
        {
            get
            {
                return (HostMappingElement)BaseGet(targetHost);
            }
        }

        public int IndexOf(HostMappingElement hostMapping)
        {
            return BaseIndexOf(hostMapping);
        }

        public void Add(HostMappingElement hostMapping)
        {
            BaseAdd(hostMapping);
        }
        protected override void BaseAdd(ConfigurationElement element)
        {
            BaseAdd(element, false);
        }

        public void Remove(HostMappingElement hostMapping)
        {
            if (BaseIndexOf(hostMapping) >= 0)
            {
                BaseRemove(hostMapping.TargetHost);
            }
        }

        public void RemoveAt(int index)
        {
            BaseRemoveAt(index);
        }

        public void Remove(string targetHost)
        {
            BaseRemove(targetHost);
        }

        public void Clear()
        {
            BaseClear();
        }
    }
}
