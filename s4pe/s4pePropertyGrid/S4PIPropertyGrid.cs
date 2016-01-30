/***************************************************************************
 *  Copyright (C) 2009, 2010 by Peter L Jones                              *
 *  pljones@users.sf.net                                                   *
 *                                                                         *
 *  This file is part of the Sims 3 Package Interface (s3pi)               *
 *                                                                         *
 *  s3pi is free software: you can redistribute it and/or modify           *
 *  it under the terms of the GNU General Public License as published by   *
 *  the Free Software Foundation, either version 3 of the License, or      *
 *  (at your option) any later version.                                    *
 *                                                                         *
 *  s3pi is distributed in the hope that it will be useful,                *
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of         *
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the          *
 *  GNU General Public License for more details.                           *
 *                                                                         *
 *  You should have received a copy of the GNU General Public License      *
 *  along with s3pi.  If not, see <http://www.gnu.org/licenses/>.          *
 ***************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using s4pi.Interfaces;

namespace S4PIDemoFE
{
    public class s4piPropertyGrid : PropertyGrid
    {
        AApiVersionedFieldsCTD target;
        public s4piPropertyGrid() : base() { HelpVisible = false; ToolbarVisible = false; }

        public AApiVersionedFields s4piObject
        {
            set
            {
                if (value != null) { target = new AApiVersionedFieldsCTD(value); SelectedObject = target; }
                else SelectedObject = null;
            }
        }
    }


    // Need to convert this to a PropertyGrid
    // http://msdn.microsoft.com/en-us/library/aa302334.aspx
    // http://www.codeproject.com/KB/tabs/customizingcollectiondata.aspx?display=Print

    [TypeConverter(typeof(AApiVersionedFieldsCTDConverter))]
    public class AApiVersionedFieldsCTD : ICustomTypeDescriptor
    {
        protected AApiVersionedFields owner;
        protected string field;
        protected object component;
        AApiVersionedFields value;
        public AApiVersionedFieldsCTD(AApiVersionedFields value) { this.value = value; }
        public AApiVersionedFieldsCTD(AApiVersionedFields owner, string field, object component) { this.owner = owner; this.field = field; this.component = component; }

        #region Helpers

        public static string GetFieldIndex(string field)
        {
            string[] split = field.Split(' ');
            return split.Length == 1 ? null : split[split[0].Trim().StartsWith("[") ? 0 : 1].Trim();
        }

        public static string GetFieldName(string field)
        {
            string[] split = field.Split(' ');
            return split.Length == 1 ? field : split[split[0].Trim().StartsWith("[") ? 1 : 0].Trim();
        }

        public static Type GetFieldType(AApiVersionedFields owner, string field)
        {
            try
            {
                Type type = AApiVersionedFields.GetContentFieldTypes(0, owner.GetType())[GetFieldName(field)];

                if (field.IndexOf(' ') == -1)
                {
	                return type;
                }
                else
                {
                    if (type.HasElementType && !type.GetElementType().IsAbstract) return type.GetElementType();

                    Type baseType = GetGenericType(type);
                    if (baseType != null && !baseType.IsAbstract) return baseType;

                    return GetFieldValue(owner, field).Type;
                }
            }
            catch (Exception ex) { throw ex; }
        }

        public static TypedValue GetFieldValue(AApiVersionedFields owner, string field)
        {
	        string index = GetFieldIndex(field);
	        if (index == null)
	        {
		        TypedValue tv = owner[field];
		        return tv.Type == typeof(Double) || tv.Type == typeof(Single) ? new TypedValue(tv.Type, tv.Value, "R") : tv;
	        }
	        else if (owner is ArrayOwner)
	        {
		        return owner[index];
	        }
	        else
	        {
		        object o = ((IGenericAdd)owner[GetFieldName(field)].Value)[Convert.ToInt32("0x" + index.Substring(1, index.Length - 2), 16)];
		        return new TypedValue(o.GetType(), o);
	        }
        }

        public static Type GetGenericType(Type type)
        {
	        Type t = type;
	        while (t.BaseType != typeof(object)) { t = t.BaseType; }
	        if (t.GetGenericArguments().Length == 1) return t.GetGenericArguments()[0];
	        
			return null;
        }

        public static bool IsCollection(Type fieldType)
        {
            if (!typeof(ICollection).IsAssignableFrom(fieldType)) return false;
            Type baseType = GetGenericType(fieldType);
            return baseType != null && typeof(AApiVersionedFields).IsAssignableFrom(baseType);
        }
        #endregion

        #region ICustomTypeDescriptor Members

        public AttributeCollection GetAttributes() { try { return TypeDescriptor.GetAttributes(this, true); } catch (Exception ex) { throw ex; } }

        public string GetClassName() { try { return TypeDescriptor.GetClassName(this, true); } catch (Exception ex) { throw ex; } }

        public string GetComponentName() { try { return TypeDescriptor.GetComponentName(this, true); } catch (Exception ex) { throw ex; } }

        public TypeConverter GetConverter() { try { return TypeDescriptor.GetConverter(this, true); } catch (Exception ex) { throw ex; } }

        public EventDescriptor GetDefaultEvent() { try { return TypeDescriptor.GetDefaultEvent(this, true); } catch (Exception ex) { throw ex; } }

        public PropertyDescriptor GetDefaultProperty() { try { return TypeDescriptor.GetDefaultProperty(this, true); } catch (Exception ex) { throw ex; } }

        public object GetEditor(Type editorBaseType) { try { return TypeDescriptor.GetEditor(this, editorBaseType, true); } catch (Exception ex) { throw ex; } }

        public EventDescriptorCollection GetEvents(Attribute[] attributes) { try { return TypeDescriptor.GetEvents(this, attributes, true); } catch (Exception ex) { throw ex; } }

        public EventDescriptorCollection GetEvents() { try { return TypeDescriptor.GetEvents(this, true); } catch (Exception ex) { throw ex; } }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            try
            {
                if (value == null) value = (AApiVersionedFields)GetFieldValue(owner, field).Value;

                List<string> filter = new List<string>(new string[] { "Stream", /*"AsBytes",/**/ "Value", });
                List<string> contentFields = value.ContentFields;
                List<TypedValuePropertyDescriptor> ltpdc = new List<TypedValuePropertyDescriptor>();
                foreach (string f in contentFields)
                {
                    if (filter.Contains(f)) continue;
                    if (!canWrite(value, f)) continue;
                    TypedValuePropertyDescriptor tvpd = new TypedValuePropertyDescriptor(value, f, null);
                    ltpdc.Add(new TypedValuePropertyDescriptor(value, f, new Attribute[] { new CategoryAttribute(tvpdToCategory(tvpd.PropertyType)) }));
                }
                List<PropertyDescriptor> lpdc = new List<PropertyDescriptor>(ltpdc.ToArray());
                int i = 0; while (i < ltpdc.Count && ltpdc[i].Priority < int.MaxValue) i++;
                if (typeof(IDictionary).IsAssignableFrom(value.GetType())) { lpdc.Insert(i, new IDictionaryPropertyDescriptor((IDictionary)value, "(this)", new Attribute[] { new CategoryAttribute("Lists") })); }
                return new PropertyDescriptorCollection(lpdc.ToArray());
            }
            catch (Exception ex) { throw ex; }
        }
        string tvpdToCategory(Type t)
        {
            if (t.Equals(typeof(EnumChooserCTD))) return "Values";
            if (t.Equals(typeof(EnumFlagsCTD))) return "Fields";
            if (t.Equals(typeof(AsHexCTD))) return "Values";
            if (t.Equals(typeof(ArrayCTD))) return "Lists";
            if (t.Equals(typeof(AApiVersionedFieldsCTD))) return "Fields";
            if (t.Equals(typeof(ICollectionAApiVersionedFieldsCTD))) return "Lists";
            if (t.Equals(typeof(TGIBlockListCTD))) return "Lists";
            if (t.Equals(typeof(IDictionaryCTD))) return "Lists";
            if (t.Equals(typeof(ReaderCTD))) return "Readers";
            return "Values";
        }
        bool canWrite(AApiVersionedFields owner, string field)
        {
            if (owner.GetType().Equals(typeof(AsKVP))) return true;
            if (owner.GetType().Equals(typeof(ArrayOwner))) return true;
            return owner.GetType().GetProperty(field).CanWrite;
        }

        public PropertyDescriptorCollection GetProperties() { try { return TypeDescriptor.GetProperties(new Attribute[] { }); } catch (Exception ex) { throw ex; } }

        public object GetPropertyOwner(PropertyDescriptor pd) { return this; }

        #endregion

        public class IDictionaryPropertyDescriptor : PropertyDescriptor
        {
            IDictionary value;
            public IDictionaryPropertyDescriptor(IDictionary value, string field, Attribute[] attrs) : base(field, attrs) { this.value = value; }

            public override bool CanResetValue(object component) { return false; }

            public override Type ComponentType { get { throw new NotImplementedException(); } }

            public override object GetValue(object component) { return new IDictionaryCTD(value); }

            public override bool IsReadOnly { get { return false; } }

            public override Type PropertyType { get { return typeof(IDictionaryCTD); } }

            public override void ResetValue(object component) { throw new NotImplementedException(); }

            public override void SetValue(object component, object value) { }

            public override bool ShouldSerializeValue(object component) { return true; }
        }

        public class TypedValuePropertyDescriptor : PropertyDescriptor
        {
            AApiVersionedFields owner;
            int priority = int.MaxValue;
            DependentList<TGIBlock> tgiBlocks = null;
            Type fieldType;
            public TypedValuePropertyDescriptor(AApiVersionedFields owner, string field, Attribute[] attrs)
                : base(field, attrs)
            {
                try
                {
                    this.owner = owner;
                    if (typeof(ArrayOwner).Equals(owner.GetType())) fieldType = ((ArrayOwner)owner).ElementType;
                    else if (typeof(AsKVP).Equals(owner.GetType())) fieldType = ((AsKVP)owner).GetType(Name);
                    else
                    {
                        string name = GetFieldName(field);
                        fieldType = GetFieldType(owner, field);
                        priority = ElementPriorityAttribute.GetPriority(owner.GetType(), name);
                        tgiBlocks = owner.GetTGIBlocks(name);
                    }
                }
                catch (Exception ex) { throw ex; }
            }

            public int Priority { get { return priority; } }

            public bool hasTGIBlocks { get { return tgiBlocks != null; } }
            public DependentList<TGIBlock> TGIBlocks { get { return tgiBlocks; } }

            public Type FieldType { get { return fieldType; } }

            public override bool CanResetValue(object component) { return false; }

            public override Type ComponentType { get { throw new NotImplementedException(); } }

            public override object GetValue(object component)
            {
                try
                {
                    Type t = PropertyType;
                    if (t.Equals(typeof(ReaderCTD))) return new ReaderCTD(owner, Name, component);
                    if (t.Equals(typeof(IDictionaryCTD))) return new IDictionaryCTD(owner, Name, component);
                    if (t.Equals(typeof(ICollectionAApiVersionedFieldsCTD))) return new ICollectionAApiVersionedFieldsCTD(owner, Name, component);
                    if (t.Equals(typeof(TGIBlockListCTD))) return new TGIBlockListCTD(owner, Name, component);
                    if (t.Equals(typeof(ArrayCTD))) return new ArrayCTD(owner, Name, component);
                    if (t.Equals(typeof(AApiVersionedFieldsCTD))) return new AApiVersionedFieldsCTD(owner, Name, component);
                    if (t.Equals(typeof(IResourceKeyCTD))) return new IResourceKeyCTD(owner, Name, component);
                    if (t.Equals(typeof(TGIBlockListIndexCTD))) return new TGIBlockListIndexCTD(owner, Name, tgiBlocks, component);
                    if (t.Equals(typeof(AsHexCTD))) return new AsHexCTD(owner, Name, component);
                    if (t.Equals(typeof(EnumChooserCTD))) return new EnumChooserCTD(owner, Name, component);
                    if (t.Equals(typeof(EnumFlagsCTD))) return new EnumFlagsCTD(owner, Name, component);
                    return GetFieldValue(owner, Name);
                }
                catch (Exception ex) { throw ex; }
            }

            public override bool IsReadOnly
            {
                get
                {
                    if (owner.GetType().Equals(typeof(ArrayOwner))) return false;
                    if (owner.GetType().Equals(typeof(AsKVP))) return false;
                    string name = Name.Split(' ').Length == 1 ? Name : Name.Split(new char[] { ' ' }, 2)[1].Trim();
                    return !owner.GetType().GetProperty(name).CanWrite; 
                }
            }

            public override Type PropertyType
            {
                get
                {
                    try
                    {
                        // Must test these before IConvertible
                        List<Type> simpleTypes = new List<Type>(new Type[] { typeof(bool), typeof(DateTime), typeof(decimal), typeof(double), typeof(float), typeof(string), });
                        if (simpleTypes.Contains(fieldType)) return fieldType;

                        // Must test enum before IConvertible
                        if (typeof(Enum).IsAssignableFrom(fieldType))
                            return fieldType.GetCustomAttributes(typeof(FlagsAttribute), true).Length == 0 ? typeof(EnumChooserCTD) : typeof(EnumFlagsCTD);

                        if (typeof(IConvertible).IsAssignableFrom(fieldType))
                            return hasTGIBlocks ? typeof(TGIBlockListIndexCTD) : typeof(AsHexCTD);

                        if (typeof(IResourceKey).IsAssignableFrom(fieldType)) return typeof(IResourceKeyCTD);

                        if (typeof(AApiVersionedFields).IsAssignableFrom(fieldType)) return typeof(AApiVersionedFieldsCTD);


                        // More complex stuff

                        // Arrays
                        if (fieldType.HasElementType
                            && (typeof(IConvertible).IsAssignableFrom(fieldType.GetElementType())
                            || typeof(AApiVersionedFields).IsAssignableFrom(fieldType.GetElementType())
                            ))
                            return typeof(ArrayCTD);

                        if (IsCollection(fieldType))
                        {
                            if (GetGenericType(fieldType) == typeof(TGIBlock))
                                return typeof(TGIBlockListCTD);

                            return typeof(ICollectionAApiVersionedFieldsCTD);
                        }

                        if (typeof(IDictionary).IsAssignableFrom(fieldType)) return typeof(IDictionaryCTD);

                        if (typeof(BinaryReader).IsAssignableFrom(fieldType) || typeof(TextReader).IsAssignableFrom(fieldType))
                            return typeof(ReaderCTD);

                        return fieldType;
                    }
                    catch (Exception ex) { throw ex; }
                }
            }

            public override void ResetValue(object component) { throw new NotImplementedException(); }

            public override void SetValue(object component, object value)
            {
                try
                {
                    string index = GetFieldIndex(Name);
                    if (index == null)
                        owner[Name] = new TypedValue(value.GetType(), value);
                    else if (owner is ArrayOwner)
                        owner[index] = new TypedValue(value.GetType(), value);
                    else
                        ((IGenericAdd)owner[GetFieldName(Name)].Value)[Convert.ToInt32("0x" + index.Substring(1, index.Length - 2), 16)] = value;
                    OnValueChanged(owner, EventArgs.Empty);
                }
                catch (Exception ex) { throw ex; }
            }

            public override bool ShouldSerializeValue(object component) { return true; }
        }

        public class AApiVersionedFieldsCTDConverter : ExpandableObjectConverter
        {
            public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            {
                if (typeof(string) == destinationType)
                {
	                return true;
                }
                return base.CanConvertTo(context, destinationType);
            }

            public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
            {
                if (typeof(string) == destinationType)
                {
	                return "";
                }
                return base.ConvertTo(context, culture, value, destinationType);
            }
        }
    }

    [Editor(typeof(TGIBlockListIndexEditor), typeof(UITypeEditor))]
    [TypeConverter(typeof(TGIBlockListIndexConverter))]
    public class TGIBlockListIndexCTD : ICustomTypeDescriptor
    {
        protected AApiVersionedFields owner;
        protected string field;
        protected DependentList<TGIBlock> tgiBlocks;
        protected object component;
        public TGIBlockListIndexCTD(AApiVersionedFields owner, string field, DependentList<TGIBlock> tgiBlocks, object component)
        {
            this.owner = owner;
            this.field = field;
            this.tgiBlocks = tgiBlocks;
            this.component = component;
        }

        #region ICustomTypeDescriptor Members

        public AttributeCollection GetAttributes() { return TypeDescriptor.GetAttributes(this, true); }

        public string GetClassName() { return TypeDescriptor.GetClassName(this, true); }

        public string GetComponentName() { return TypeDescriptor.GetComponentName(this, true); }

        public TypeConverter GetConverter() { return TypeDescriptor.GetConverter(this, true); }

        public EventDescriptor GetDefaultEvent() { return TypeDescriptor.GetDefaultEvent(this, true); }

        public System.ComponentModel.PropertyDescriptor GetDefaultProperty() { return TypeDescriptor.GetDefaultProperty(this, true); }

        public object GetEditor(Type editorBaseType) { return TypeDescriptor.GetEditor(this, editorBaseType, true); }

        public EventDescriptorCollection GetEvents(Attribute[] attributes) { return TypeDescriptor.GetEvents(this, attributes, true); }

        public EventDescriptorCollection GetEvents() { return TypeDescriptor.GetEvents(this, true); }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes) { return new PropertyDescriptorCollection(new PropertyDescriptor[] { new ConverterPropertyDescriptor(owner, field, component, null), }); }

        public PropertyDescriptorCollection GetProperties() { return GetProperties(null); }

        public object GetPropertyOwner(System.ComponentModel.PropertyDescriptor pd) { return this; }

        #endregion

        public class TGIBlockListIndexEditor : UITypeEditor
        {
            TGIBlockSelection ui;
            public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) { return UITypeEditorEditStyle.DropDown; }
            public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
            {
                IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

                if (ui == null)
                    ui = new TGIBlockSelection();

                TGIBlockListIndexCTD o = value as TGIBlockListIndexCTD;
                ui.SetField(o.owner, o.field, o.tgiBlocks);
                ui.EdSvc = edSvc;
                edSvc.DropDownControl(ui);
                // the ui (a) updates the value and (b) closes the dropdown

                return o.owner[o.field].Value;
            }
        }

        public class TGIBlockListIndexConverter : TypeConverter
        {
            public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            {
                if (sourceType.Equals(typeof(string))) return true;
                return base.CanConvertFrom(context, sourceType);
            }

            public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
            {
                if (value != null && value.GetType().Equals(typeof(string)))
                {
                    string str = ((string)value).Trim();
                    try
                    {
                        AApiVersionedFieldsCTD.TypedValuePropertyDescriptor pd = (AApiVersionedFieldsCTD.TypedValuePropertyDescriptor)context.PropertyDescriptor;
                        ulong num = Convert.ToUInt64(str, str.StartsWith("0x", StringComparison.InvariantCultureIgnoreCase) ? 16 : 10);
                        return Convert.ChangeType(num, pd.FieldType);
                    }
                    catch (Exception ex) { throw new NotSupportedException("Invalid data: " + str, ex); }
                }
                return base.ConvertFrom(context, culture, value);
            }/**/

            public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            {
                if (destinationType.Equals(typeof(string))) return true;
                return base.CanConvertTo(context, destinationType);
            }

            public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
            {
                if (value as TGIBlockListIndexCTD != null && destinationType.Equals(typeof(string)))
                {
                    try
                    {
                        TGIBlockListIndexCTD ctd = (TGIBlockListIndexCTD)value;
                        string name = ctd.field.Split(' ').Length == 1 ? ctd.field : ctd.field.Split(new char[] { ' ' }, 2)[1].Trim();
                        return "" + ctd.owner[name];
                    }
                    //catch { }
                    catch (Exception ex) { throw new NotSupportedException("Invalid data", ex); }
                }
                return base.ConvertTo(context, culture, value, destinationType);
            }
        }
    }

    [TypeConverter(typeof(AsHexConverter))]
    public class AsHexCTD : ICustomTypeDescriptor
    {
        protected AApiVersionedFields owner;
        protected string field;
        protected object component;
        public AsHexCTD(AApiVersionedFields owner, string field, object component) { this.owner = owner; this.field = field; this.component = component; }

        #region ICustomTypeDescriptor Members

        public AttributeCollection GetAttributes() { return TypeDescriptor.GetAttributes(this, true); }

        public string GetClassName() { return TypeDescriptor.GetClassName(this, true); }

        public string GetComponentName() { return TypeDescriptor.GetComponentName(this, true); }

        public TypeConverter GetConverter() { return TypeDescriptor.GetConverter(this, true); }

        public EventDescriptor GetDefaultEvent() { return TypeDescriptor.GetDefaultEvent(this, true); }

        public System.ComponentModel.PropertyDescriptor GetDefaultProperty() { return TypeDescriptor.GetDefaultProperty(this, true); }

        public object GetEditor(Type editorBaseType) { return TypeDescriptor.GetEditor(this, editorBaseType, true); }

        public EventDescriptorCollection GetEvents(Attribute[] attributes) { return TypeDescriptor.GetEvents(this, attributes, true); }

        public EventDescriptorCollection GetEvents() { return TypeDescriptor.GetEvents(this, true); }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes) { return new PropertyDescriptorCollection(new PropertyDescriptor[] { new ConverterPropertyDescriptor(owner, field, component, null), }); }

        public PropertyDescriptorCollection GetProperties() { return GetProperties(null); }

        public object GetPropertyOwner(System.ComponentModel.PropertyDescriptor pd) { return this; }

        #endregion

        public class AsHexConverter : TypeConverter
        {
            public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            {
                if (sourceType.Equals(typeof(string))) return true;
                return base.CanConvertFrom(context, sourceType);
            }

            public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
            {
                if (value != null && value.GetType().Equals(typeof(string)))
                {
                    string str = ((string)value).Trim();
                    try
                    {
                        AApiVersionedFieldsCTD.TypedValuePropertyDescriptor pd = (AApiVersionedFieldsCTD.TypedValuePropertyDescriptor)context.PropertyDescriptor;
                        ulong num = Convert.ToUInt64(str, str.StartsWith("0x", StringComparison.InvariantCultureIgnoreCase) ? 16 : 10);
                        return Convert.ChangeType(num, pd.FieldType);
                    }
                    catch (Exception ex) { throw new NotSupportedException("Invalid data: " + str, ex); }
                }
                return base.ConvertFrom(context, culture, value);
            }

            public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            {
                if (destinationType.Equals(typeof(string))) return true;
                return base.CanConvertTo(context, destinationType);
            }

            public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
            {
                if (value as AsHexCTD != null && destinationType.Equals(typeof(string)))
                {
                    try
                    {
                        AsHexCTD ctd = (AsHexCTD)value;
                        string name = ctd.field.Split(' ').Length == 1 ? ctd.field : ctd.field.Split(new char[] { ' ' }, 2)[1].Trim();
                        return "" + ctd.owner[name];
                    }
                    catch (Exception ex) { throw new NotSupportedException("Invalid data", ex); }
                }
                return base.ConvertTo(context, culture, value, destinationType);
            }
        }
    }

    [Editor(typeof(EnumChooserEditor), typeof(UITypeEditor))]
    [TypeConverter(typeof(EnumChooserConverter))]
    public class EnumChooserCTD : ICustomTypeDescriptor
    {
        protected AApiVersionedFields owner;
        protected string field;
        protected object component;
        public EnumChooserCTD(AApiVersionedFields owner, string field, object component) { this.owner = owner; this.field = field; this.component = component; }

        #region ICustomTypeDescriptor Members

        public AttributeCollection GetAttributes() { return TypeDescriptor.GetAttributes(this, true); }

        public string GetClassName() { return TypeDescriptor.GetClassName(this, true); }

        public string GetComponentName() { return TypeDescriptor.GetComponentName(this, true); }

        public TypeConverter GetConverter() { return TypeDescriptor.GetConverter(this, true); }

        public EventDescriptor GetDefaultEvent() { return TypeDescriptor.GetDefaultEvent(this, true); }

        public System.ComponentModel.PropertyDescriptor GetDefaultProperty() { return TypeDescriptor.GetDefaultProperty(this, true); }

        public object GetEditor(Type editorBaseType) { return TypeDescriptor.GetEditor(this, editorBaseType, true); }

        public EventDescriptorCollection GetEvents(Attribute[] attributes) { return TypeDescriptor.GetEvents(this, attributes, true); }

        public EventDescriptorCollection GetEvents() { return TypeDescriptor.GetEvents(this, true); }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes) { return new PropertyDescriptorCollection(new PropertyDescriptor[] { new ConverterPropertyDescriptor(owner, field, component, null), }); }

        public PropertyDescriptorCollection GetProperties() { return GetProperties(null); }

        public object GetPropertyOwner(System.ComponentModel.PropertyDescriptor pd) { return this; }

        #endregion

        public class EnumChooserConverter : TypeConverter
        {
            public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            {
                if (sourceType.Equals(typeof(string))) return true;
                return base.CanConvertFrom(context, sourceType);
            }
            public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
            {
                if (value != null && value.GetType().Equals(typeof(string)))
                {
                    AApiVersionedFieldsCTD.TypedValuePropertyDescriptor pd = (AApiVersionedFieldsCTD.TypedValuePropertyDescriptor)context.PropertyDescriptor;
                    string v = value as string;
                    try
                    {
                        if (v.Split(' ').Length > 1) v = v.Split(' ')[0];
                        if (new List<string>(Enum.GetNames(pd.FieldType)).Contains(v))
                            return Enum.Parse(pd.FieldType, v);
                        return Enum.ToObject(pd.FieldType, Convert.ToUInt64(v, v.StartsWith("0x") ? 16 : 10));
                    }
                    catch (Exception ex) { throw new NotSupportedException("Invalid data: " + v, ex); }
                }
                return base.ConvertFrom(context, culture, value);
            }

            public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            {
                if (destinationType.Equals(typeof(string))) return true;
                return base.CanConvertTo(context, destinationType);
            }
            public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
            {
                if (value as EnumChooserCTD != null && destinationType.Equals(typeof(string)))
                {
                    try
                    {
                        EnumChooserCTD ctd = (EnumChooserCTD)value;
                        TypedValue tv = (TypedValue)AApiVersionedFieldsCTD.GetFieldValue(ctd.owner, ctd.field);
                        return "" + AApiVersionedFieldsCTD.GetFieldValue(ctd.owner, ctd.field);
                    }
                    catch (Exception ex) { throw new NotSupportedException("Invalid data", ex); }
                }
                return base.ConvertTo(context, culture, value, destinationType);
            }
        }

        public class EnumChooserEditor : UITypeEditor
        {
            public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) { return UITypeEditorEditStyle.DropDown; }

            public override bool IsDropDownResizable { get { return true; } }

            public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
            {
                IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

                EnumChooserCTD ctd = (EnumChooserCTD)value;
                TypedValue typedValue = AApiVersionedFieldsCTD.GetFieldValue(ctd.owner, ctd.field);

                List<string> enumValues = new List<string>();
                int index = -1;
                int i = 0;
                foreach (Enum e in Enum.GetValues(typedValue.Type))
                {
                    if (e.Equals((Enum)typedValue.Value)) index = i;
                    enumValues.Add(new TypedValue(e.GetType(), e, "X"));
                    i++;
                }

                int maxWidth = Application.OpenForms[0].Width / 3;
                if (maxWidth < Screen.PrimaryScreen.WorkingArea.Size.Width / 4) maxWidth = Screen.PrimaryScreen.WorkingArea.Size.Width / 4;
                int maxHeight = Application.OpenForms[0].Height / 3;
                if (maxHeight < Screen.PrimaryScreen.WorkingArea.Size.Height / 4) maxHeight = Screen.PrimaryScreen.WorkingArea.Size.Height / 4;

                TextBox tb = new TextBox
                {
                    AutoSize = true,
                    Font = new ListBox().Font,
                    Margin = new Padding(0),
                    MaximumSize = new System.Drawing.Size(maxWidth, maxHeight),
                    Multiline = true,
                    Lines = enumValues.ToArray(),
                };
                tb.PerformLayout();

                ListBox lb = new ListBox()
                {
                    IntegralHeight = false,
                    Margin = new Padding(0),
                    Size = tb.PreferredSize,
                    Tag = edSvc,
                };
                lb.Items.AddRange(enumValues.ToArray());
                lb.PerformLayout();

                if (index >= 0) { lb.SelectedIndices.Add(index); }
                lb.SelectedIndexChanged += new EventHandler(lb_SelectedIndexChanged);
                edSvc.DropDownControl(lb);

                return lb.SelectedItem == null ? value : (Enum)new EnumChooserConverter().ConvertFrom(context, System.Globalization.CultureInfo.CurrentCulture, lb.SelectedItem);
            }

            void lb_SelectedIndexChanged(object sender, EventArgs e) { ((sender as ListBox).Tag as IWindowsFormsEditorService).CloseDropDown(); }
        }
    }

    [TypeConverter(typeof(EnumFlagsConverter))]
    public class EnumFlagsCTD : ICustomTypeDescriptor
    {
        protected AApiVersionedFields owner;
        protected string field;
        protected object component;
        public EnumFlagsCTD(AApiVersionedFields owner, string field, object component) { this.owner = owner; this.field = field; this.component = component; }

        #region ICustomTypeDescriptor Members

        public AttributeCollection GetAttributes() { return TypeDescriptor.GetAttributes(this, true); }

        public string GetClassName() { return TypeDescriptor.GetClassName(this, true); }

        public string GetComponentName() { return TypeDescriptor.GetComponentName(this, true); }

        public TypeConverter GetConverter() { return TypeDescriptor.GetConverter(this, true); }

        public EventDescriptor GetDefaultEvent() { return TypeDescriptor.GetDefaultEvent(this, true); }

        public System.ComponentModel.PropertyDescriptor GetDefaultProperty() { return TypeDescriptor.GetDefaultProperty(this, true); }

        public object GetEditor(Type editorBaseType) { return TypeDescriptor.GetEditor(this, editorBaseType, true); }

        public EventDescriptorCollection GetEvents(Attribute[] attributes) { return TypeDescriptor.GetEvents(this, attributes, true); }

        public EventDescriptorCollection GetEvents() { return TypeDescriptor.GetEvents(this, true); }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes) { return new PropertyDescriptorCollection(new PropertyDescriptor[] { new ConverterPropertyDescriptor(owner, field, component, null), }); }

        public PropertyDescriptorCollection GetProperties() { return GetProperties(null); }

        public object GetPropertyOwner(System.ComponentModel.PropertyDescriptor pd) { return this; }

        #endregion

        public class EnumFlagsConverter : ExpandableObjectConverter
        {
            public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            {
                if (sourceType.Equals(typeof(string))) return true;
                return base.CanConvertTo(context, sourceType);
            }

            public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
            {
                if (value != null && value.GetType().Equals(typeof(string)))
                {
                    string[] content = ((string)value).Split(new char[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
                    string str = content[0];
                    try
                    {
                        AApiVersionedFieldsCTD.TypedValuePropertyDescriptor pd = (AApiVersionedFieldsCTD.TypedValuePropertyDescriptor)context.PropertyDescriptor;
                        ulong num = Convert.ToUInt64(str, str.StartsWith("0x", StringComparison.InvariantCultureIgnoreCase) ? 16 : 10);
                        return Enum.ToObject(pd.FieldType, num);
                    }
                    catch (Exception ex) { throw new NotSupportedException("Invalid data: " + str, ex); }
                }
                return base.ConvertFrom(context, culture, value);
            }

            public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            {
                if (destinationType.Equals(typeof(string))) return true;
                return base.CanConvertTo(context, destinationType);
            }

            public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
            {
                if (value as EnumFlagsCTD != null && destinationType.Equals(typeof(string)))
                {
                    try
                    {
                        EnumFlagsCTD ctd = (EnumFlagsCTD)value;
                        string name = ctd.field.Split(' ').Length == 1 ? ctd.field : ctd.field.Split(new char[] { ' ' }, 2)[1].Trim();
                        TypedValue tv = ctd.owner[name];
                        return "0x" + Enum.Format(tv.Type, tv.Value, "X");
                    }
                    //catch { }
                    catch (Exception ex) { throw new NotSupportedException("Invalid data", ex); }
                }
                return base.ConvertTo(context, culture, value, destinationType);
            }

            static int underlyingTypeToBits(Type t)
            {
                if (t.Equals(typeof(byte))) return 8;
                if (t.Equals(typeof(ushort))) return 16;
                if (t.Equals(typeof(uint))) return 32;
                if (t.Equals(typeof(ulong))) return 64;
                return -1;
            }
            public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
            {
                EnumFlagsCTD ctd = (EnumFlagsCTD)value;
                string name = ctd.field.Split(' ').Length == 1 ? ctd.field : ctd.field.Split(new char[] { ' ' }, 2)[1].Trim();
                Type enumType = AApiVersionedFields.GetContentFieldTypes(0, ctd.owner.GetType())[name];
                int bits = underlyingTypeToBits(Enum.GetUnderlyingType(enumType));
                EnumFlagPropertyDescriptor[] enumFlags = new EnumFlagPropertyDescriptor[bits];
                string fmt = "[{0:X" + bits.ToString("X").Length + "}] ";
                for (int i = 0; i < bits; i++)
                {
                    ulong u = (ulong)1 << i;
                    string s = (Enum)Enum.ToObject(enumType, u) + "";
                    if (s == u.ToString()) s = "-";
                    s = String.Format(fmt, i) + s;
                    enumFlags[i] = new EnumFlagPropertyDescriptor(ctd.owner, ctd.field, u, s);
                }
                return new PropertyDescriptorCollection(enumFlags);
            }

            public class EnumFlagPropertyDescriptor : PropertyDescriptor
            {
                AApiVersionedFields owner;
                string field;
                ulong mask;
                public EnumFlagPropertyDescriptor(AApiVersionedFields owner, string field, ulong mask, string value) : base(value, null) { this.owner = owner; this.field = field; this.mask = mask; }
                public override bool CanResetValue(object component) { return true; }

                public override Type ComponentType { get { throw new NotImplementedException(); } }

                ulong getFlags(AApiVersionedFields owner, string field)
                {
                    TypedValue tv = owner[field];
                    object o = Convert.ChangeType(tv.Value, Enum.GetUnderlyingType(tv.Type));
                    if (o.GetType().Equals(typeof(byte))) return (byte)o;
                    if (o.GetType().Equals(typeof(ushort))) return (ushort)o;
                    if (o.GetType().Equals(typeof(uint))) return (uint)o;
                    return (ulong)o;
                }
                public override object GetValue(object component)
                {
                    string name = field.Split(' ').Length == 1 ? field : field.Split(new char[] { ' ' }, 2)[1].Trim();
                    ulong old = getFlags(owner, name);
                    return (old & mask) != 0;
                }

                public override bool IsReadOnly { get { return !owner.GetType().GetProperty(field).CanWrite; } }

                public override Type PropertyType { get { return typeof(Boolean); } }

                public override void ResetValue(object component) { SetValue(component, false); }

                void setFlags(AApiVersionedFields owner, string field, ulong mask, bool value)
                {
                    ulong old = getFlags(owner, field);
                    ulong res = old & ~mask;
                    if (value) res |= mask;
                    Type t = AApiVersionedFields.GetContentFieldTypes(0, owner.GetType())[field];
                    TypedValue tv = new TypedValue(t, Enum.ToObject(t, res));
                    owner[field] = tv;
                }
                public override void SetValue(object component, object value)
                {
                    setFlags(owner, field, mask, (bool)value);
                    OnValueChanged(owner, EventArgs.Empty);
                }

                public override bool ShouldSerializeValue(object component) { return false; }
            }
        }
    }

	[TypeConverter(typeof(IResourceKeyConverter))]
    public class IResourceKeyCTD : AApiVersionedFieldsCTD
    {
        public IResourceKeyCTD(AApiVersionedFields owner, string field, object component) : base(owner, field, component) { }

        public class IResourceKeyConverter : AApiVersionedFieldsCTD.AApiVersionedFieldsCTDConverter
        {
            public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            {
                if (sourceType == typeof(string) || typeof(IResourceKey).IsAssignableFrom(sourceType))
                    return true;
                return base.CanConvertTo(context, sourceType);
            }

            public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
            {
                AApiVersionedFieldsCTD.TypedValuePropertyDescriptor pd = (AApiVersionedFieldsCTD.TypedValuePropertyDescriptor)context.PropertyDescriptor;
                IResourceKeyCTD rkCTD = (IResourceKeyCTD)pd.GetValue(null);
                AApiVersionedFields owner = rkCTD.owner;
                string field = rkCTD.field;
                object component = rkCTD.component;
                IResourceKey rk = (IResourceKey)AApiVersionedFieldsCTD.GetFieldValue(owner, field).Value;

                if (typeof(IResourceKey).IsAssignableFrom(value.GetType()))
                {
                    IResourceKey rkNew = (IResourceKey)value;
                    rk.ResourceType = rkNew.ResourceType;
                    rk.ResourceGroup = rkNew.ResourceGroup;
                    rk.Instance = rkNew.Instance;
                    return rk;
                }
                if (value != null && value is string)
                {
                    if (AResourceKey.TryParse((string)value, rk))
                        return rk;
                    else
                        throw new NotSupportedException("Invalid data: " + (string)value);
                }
                return base.ConvertFrom(context, culture, value);
            }

            public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            {
                if (destinationType.Equals(typeof(string))) return true;
                return base.CanConvertTo(context, destinationType);
            }

            public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
            {
                if (destinationType.Equals(typeof(string)))
                {
                    IResourceKeyCTD ctd = value as IResourceKeyCTD;
                    IResourceKey rk = (IResourceKey)GetFieldValue(ctd.owner, ctd.field).Value;
                    return "" + rk;
                }
                return base.ConvertTo(context, culture, value, destinationType);
            }
        }
    }

    [Editor(typeof(ICollectionAApiVersionedFieldsEditor), typeof(UITypeEditor))]
    [TypeConverter(typeof(ICollectionAApiVersionedFieldsConverter))]
    public class ICollectionAApiVersionedFieldsCTD : ICustomTypeDescriptor
    {
        protected AApiVersionedFields owner;
        protected string field;
        protected object component;
        public ICollectionAApiVersionedFieldsCTD(AApiVersionedFields owner, string field, object component) { this.owner = owner; this.field = field; this.component = component; }

        #region ICustomTypeDescriptor Members

        public AttributeCollection GetAttributes() { return TypeDescriptor.GetAttributes(this, true); }

        public string GetClassName() { return TypeDescriptor.GetClassName(this, true); }

        public string GetComponentName() { return TypeDescriptor.GetComponentName(this, true); }

        public TypeConverter GetConverter() { return TypeDescriptor.GetConverter(this, true); }

        public EventDescriptor GetDefaultEvent() { return TypeDescriptor.GetDefaultEvent(this, true); }

        public System.ComponentModel.PropertyDescriptor GetDefaultProperty() { return TypeDescriptor.GetDefaultProperty(this, true); }

        public object GetEditor(Type editorBaseType) { return TypeDescriptor.GetEditor(this, editorBaseType, true); }

        public EventDescriptorCollection GetEvents(Attribute[] attributes) { return TypeDescriptor.GetEvents(this, attributes, true); }

        public EventDescriptorCollection GetEvents() { return TypeDescriptor.GetEvents(this, true); }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            return new PropertyDescriptorCollection(new PropertyDescriptor[] { new ConverterPropertyDescriptor(owner, field, component, null), });
        }

        public PropertyDescriptorCollection GetProperties() { return GetProperties(null); }

        public object GetPropertyOwner(System.ComponentModel.PropertyDescriptor pd) { return this; }

        #endregion

        public class ICollectionAApiVersionedFieldsEditor : UITypeEditor
        {
            public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) { return UITypeEditorEditStyle.Modal; }
            public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
            {
                IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
                ICollectionAApiVersionedFieldsCTD field = (ICollectionAApiVersionedFieldsCTD)value;
                NewGridForm ui = new NewGridForm((IGenericAdd)field.owner[field.field].Value);
                edSvc.ShowDialog(ui);

                return field.owner[field.field].Value;
            }
        }

        public class ICollectionAApiVersionedFieldsConverter : ExpandableObjectConverter
        {
            public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            {
                if (typeof(string).Equals(destinationType)) return true;
                return base.CanConvertTo(context, destinationType);
            }
            public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
            {
                ICollectionAApiVersionedFieldsCTD ctd = value as ICollectionAApiVersionedFieldsCTD;
                ICollection ary = (ICollection)ctd.owner[ctd.field].Value;

                if (typeof(string).Equals(destinationType)) return ary == null ? "(null)" : "(Collection: 0x" + ary.Count.ToString("X") + ")";
                return base.ConvertTo(context, culture, value, destinationType);
            }

            public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
            {
                ICollectionAApiVersionedFieldsCTD ctd = value as ICollectionAApiVersionedFieldsCTD;
                ICollection ary = (ICollection)ctd.owner[ctd.field].Value;

                AApiVersionedFieldsCTD.TypedValuePropertyDescriptor[] pds = new AApiVersionedFieldsCTD.TypedValuePropertyDescriptor[ary.Count];
                string fmt = "[{0:X" + ary.Count.ToString("X").Length + "}] {1}";
                for (int i = 0; i < ary.Count; i++)
                {
                    try
                    {
                        pds[i] = new AApiVersionedFieldsCTD.TypedValuePropertyDescriptor(ctd.owner, String.Format(fmt, i, ctd.field), new Attribute[] { });
                    }
                    catch (Exception ex) { throw ex; }
                }
                return new PropertyDescriptorCollection(pds);
            }
        }
    }

    [TypeConverter(typeof(ArrayConverter))]
    public class ArrayCTD : ICustomTypeDescriptor
    {
        protected AApiVersionedFields owner;
        protected string field;
        protected object component;
        public ArrayCTD(AApiVersionedFields owner, string field, object component) { this.owner = owner; this.field = field; this.component = component; }

        #region ICustomTypeDescriptor Members

        public AttributeCollection GetAttributes() { return TypeDescriptor.GetAttributes(this, true); }

        public string GetClassName() { return TypeDescriptor.GetClassName(this, true); }

        public string GetComponentName() { return TypeDescriptor.GetComponentName(this, true); }

        public TypeConverter GetConverter() { return TypeDescriptor.GetConverter(this, true); }

        public EventDescriptor GetDefaultEvent() { return TypeDescriptor.GetDefaultEvent(this, true); }

        public System.ComponentModel.PropertyDescriptor GetDefaultProperty() { return TypeDescriptor.GetDefaultProperty(this, true); }

        public object GetEditor(Type editorBaseType) { return TypeDescriptor.GetEditor(this, editorBaseType, true); }

        public EventDescriptorCollection GetEvents(Attribute[] attributes) { return TypeDescriptor.GetEvents(this, attributes, true); }

        public EventDescriptorCollection GetEvents() { return TypeDescriptor.GetEvents(this, true); }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            return new PropertyDescriptorCollection(new PropertyDescriptor[] { new ConverterPropertyDescriptor(owner, field, component, null), });
        }

        public PropertyDescriptorCollection GetProperties() { return GetProperties(null); }

        public object GetPropertyOwner(System.ComponentModel.PropertyDescriptor pd) { return this; }

        #endregion

        public Array Value { get { return (Array)owner[field].Value; } }

        public class ArrayConverter : ExpandableObjectConverter
        {
            public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            {
                if (typeof(string).Equals(destinationType)) return true;
                return base.CanConvertTo(context, destinationType);
            }
            public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
            {
                ArrayCTD ctd = value as ArrayCTD;
                Array ary = ctd.owner[ctd.field].Value as Array;

                if (typeof(string).Equals(destinationType)) return ary == null ? "(null)" : "(Array: 0x" + ary.Length.ToString("X") + ")";
                return base.ConvertTo(context, culture, value, destinationType);
            }

            public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
            {
                ArrayCTD ctd = value as ArrayCTD;
                AApiVersionedFields owner = new ArrayOwner(ctd.owner, ctd.field);
                Array ary = ctd.Value;
                Type type = ary.GetType().GetElementType();
                string fmt = type.Name + " [{0:X" + ary.Length.ToString("X").Length + "}]";

                AApiVersionedFieldsCTD.TypedValuePropertyDescriptor[] pds = new AApiVersionedFieldsCTD.TypedValuePropertyDescriptor[ary.Length];
                for (int i = 0; i < ary.Length; i++)
                {
                    try
                    {
                        pds[i] = new AApiVersionedFieldsCTD.TypedValuePropertyDescriptor(owner, String.Format(fmt, i), new Attribute[] { });
                    }
                    catch (Exception ex) { throw ex; }
                }
                return new PropertyDescriptorCollection(pds);
            }
        }
    }

    [Editor(typeof(IDictionaryEditor), typeof(UITypeEditor))]
    [TypeConverter(typeof(IDictionaryConverter))]
    public class IDictionaryCTD : ICustomTypeDescriptor
    {
        protected AApiVersionedFields owner;
        protected string field;
        protected object component;
        IDictionary value;
        public IDictionaryCTD(AApiVersionedFields owner, string field, object component) { this.owner = owner; this.field = field; this.component = component; }
        public IDictionaryCTD(IDictionary value) { this.value = value; }

        public IDictionary Value { get { if (value == null) value = (IDictionary)owner[field].Value; return value; } }

        #region ICustomTypeDescriptor Members

        public AttributeCollection GetAttributes() { return TypeDescriptor.GetAttributes(this, true); }

        public string GetClassName() { return TypeDescriptor.GetClassName(this, true); }

        public string GetComponentName() { return TypeDescriptor.GetComponentName(this, true); }

        public TypeConverter GetConverter() { return TypeDescriptor.GetConverter(this, true); }

        public EventDescriptor GetDefaultEvent() { return TypeDescriptor.GetDefaultEvent(this, true); }

        public System.ComponentModel.PropertyDescriptor GetDefaultProperty() { return TypeDescriptor.GetDefaultProperty(this, true); }

        public object GetEditor(Type editorBaseType) { return TypeDescriptor.GetEditor(this, editorBaseType, true); }

        public EventDescriptorCollection GetEvents(Attribute[] attributes) { return TypeDescriptor.GetEvents(this, attributes, true); }

        public EventDescriptorCollection GetEvents() { return TypeDescriptor.GetEvents(this, true); }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            return new PropertyDescriptorCollection(new PropertyDescriptor[] { new ConverterPropertyDescriptor(owner, field, component, null), });
        }

        public PropertyDescriptorCollection GetProperties() { return GetProperties(null); }

        public object GetPropertyOwner(System.ComponentModel.PropertyDescriptor pd) { return this; }

        #endregion

        public class IDictionaryEditor : UITypeEditor
        {
            DictionaryEntry getDefault(Type kvt)
            {
                var getter = kvt.GetMethod("GetDefault");
                if (getter != null)
                    return (DictionaryEntry)getter.Invoke(null, null);
                throw new ArgumentException(string.Format("{0} does not have method 'GetDefault'.", kvt.Name));
            }

            public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) { return UITypeEditorEditStyle.Modal; }
            public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
            {
                IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

                IDictionaryCTD field = (IDictionaryCTD)value;
                if (field.Value == null) return value;

                Type[] interfaces = field.Value.GetType().GetInterfaces().Where(x => x.Name == "IDictionary`2").ToArray();
                if (interfaces.Length != 1) return value;
                DictionaryEntry entry = getDefault(field.Value.GetType());

                List<object> oldKeys = new List<object>();
                AsKVPList list = new AsKVPList(entry);
                foreach (var k in field.Value.Keys) { list.Add(new AsKVP(0, null, k, field.Value[k])); oldKeys.Add(k); }

                NewGridForm ui = new NewGridForm(list);
                edSvc.ShowDialog(ui);

                List<object> newKeys = new List<object>();
                foreach (AsKVP kvp in list)
                    newKeys.Add(kvp["Key"].Value);

                List<object> delete = new List<object>();
                foreach (var k in field.Value.Keys) if (!newKeys.Contains(k)) delete.Add(k);
                foreach (object k in delete) field.Value.Remove(k);

                bool dups = false;
                foreach (AsKVP kvp in list)
                    if (oldKeys.Contains(kvp["Key"].Value)) field.Value[kvp["Key"].Value] = kvp["Val"].Value;
                    else if (!field.Value.Contains(kvp["Key"].Value)) field.Value.Add(kvp["Key"].Value, kvp["Val"].Value);
                    else dups = true;
                if (dups)
                    CopyableMessageBox.Show("Duplicate keyed entries were dropped.", "s3pe", CopyableMessageBoxButtons.OK, CopyableMessageBoxIcon.Warning);

                return value;
            }
        }

        public class IDictionaryConverter : TypeConverter
        {
            public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            {
                if (typeof(string).Equals(destinationType)) return true;
                return base.CanConvertTo(context, destinationType);
            }
            public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
            {
                IDictionaryCTD ctd = value as IDictionaryCTD;
                IDictionary id = (IDictionary)ctd.Value;

                if (typeof(string).Equals(destinationType)) return id == null ? "(null)" : "(Dictionary: 0x" + id.Count.ToString("X") + ")";
                return base.ConvertTo(context, culture, value, destinationType);
            }
        }
    }

    [Editor(typeof(TGIBlockListEditor), typeof(UITypeEditor))]
    [TypeConverter(typeof(TGIBlockListConverter))]
    public class TGIBlockListCTD : ICustomTypeDescriptor
    {
        protected AApiVersionedFields owner;
        protected string field;
        protected object component;
        public TGIBlockListCTD(AApiVersionedFields owner, string field, object component) { this.owner = owner; this.field = field; this.component = component; }

        #region ICustomTypeDescriptor Members

        public AttributeCollection GetAttributes() { return TypeDescriptor.GetAttributes(this, true); }

        public string GetClassName() { return TypeDescriptor.GetClassName(this, true); }

        public string GetComponentName() { return TypeDescriptor.GetComponentName(this, true); }

        public TypeConverter GetConverter() { return TypeDescriptor.GetConverter(this, true); }

        public EventDescriptor GetDefaultEvent() { return TypeDescriptor.GetDefaultEvent(this, true); }

        public System.ComponentModel.PropertyDescriptor GetDefaultProperty() { return TypeDescriptor.GetDefaultProperty(this, true); }

        public object GetEditor(Type editorBaseType) { return TypeDescriptor.GetEditor(this, editorBaseType, true); }

        public EventDescriptorCollection GetEvents(Attribute[] attributes) { return TypeDescriptor.GetEvents(this, attributes, true); }

        public EventDescriptorCollection GetEvents() { return TypeDescriptor.GetEvents(this, true); }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes) { return new PropertyDescriptorCollection(new PropertyDescriptor[] { new ConverterPropertyDescriptor(owner, field, component, null), }); }

        public PropertyDescriptorCollection GetProperties() { return GetProperties(null); }

        public object GetPropertyOwner(System.ComponentModel.PropertyDescriptor pd) { return this; }

        #endregion

        public class TGIBlockListEditor : UITypeEditor
        {
            System.Windows.Forms.TGIBlockListEditorForm.MainForm ui;
            public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) { return UITypeEditorEditStyle.Modal; }
            public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
            {
                IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

                if (ui == null)
                {
                    ui = new System.Windows.Forms.TGIBlockListEditorForm.MainForm();
                    ui.Icon = ((System.Drawing.Icon)(new ComponentResourceManager(typeof(MainForm)).GetObject("$this.Icon")));
                }

                TGIBlockListCTD ctd = value as TGIBlockListCTD;
                DependentList<TGIBlock> list = ctd.owner[ctd.field].Value as DependentList<TGIBlock>;

                ui.Items = list;
                DialogResult dr = edSvc.ShowDialog(ui);

                if (dr != DialogResult.OK) return value;

                list.Clear();
                list.AddRange(ui.Items);

                return value;
            }
        }

        public class TGIBlockListConverter : ExpandableObjectConverter
        {
            public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            {
                if (typeof(string).Equals(destinationType)) return true;
                return base.CanConvertTo(context, destinationType);
            }
            public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
            {
                TGIBlockListCTD ctd = value as TGIBlockListCTD;
                DependentList<TGIBlock> list = ctd.owner[ctd.field].Value as DependentList<TGIBlock>;

                if (typeof(string).Equals(destinationType)) return list == null ? "(null)" : "(TGI Blocks: 0x" + list.Count.ToString("X") + ")";
                return base.ConvertTo(context, culture, value, destinationType);
            }

            public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
            {
                TGIBlockListCTD ctd = value as TGIBlockListCTD;
                ICollection ary = (ICollection)ctd.owner[ctd.field].Value;

                AApiVersionedFieldsCTD.TypedValuePropertyDescriptor[] pds = new AApiVersionedFieldsCTD.TypedValuePropertyDescriptor[ary.Count];
                string fmt = "[{0:X" + ary.Count.ToString("X").Length + "}] {1}";
                int i = 0;
                foreach (var o in ary)
                {
                    try
                    {
                        pds[i] = new AApiVersionedFieldsCTD.TypedValuePropertyDescriptor(ctd.owner, String.Format(fmt, i, ctd.field), new Attribute[] { });
                    }
                    catch (Exception ex) { throw ex; }
                    i++;
                }
                return new PropertyDescriptorCollection(pds);
            }
        }
    }

    [Editor(typeof(ReaderEditor), typeof(UITypeEditor))]
    [TypeConverter(typeof(ReaderConverter))]
    public class ReaderCTD : ICustomTypeDescriptor
    {
        protected AApiVersionedFields owner;
        protected string field;
        protected object component;
        public ReaderCTD(AApiVersionedFields owner, string field, object component) { this.owner = owner; this.field = field; this.component = component; }

        #region ICustomTypeDescriptor Members

        public AttributeCollection GetAttributes() { return TypeDescriptor.GetAttributes(this, true); }

        public string GetClassName() { return TypeDescriptor.GetClassName(this, true); }

        public string GetComponentName() { return TypeDescriptor.GetComponentName(this, true); }

        public TypeConverter GetConverter() { return TypeDescriptor.GetConverter(this, true); }

        public EventDescriptor GetDefaultEvent() { return TypeDescriptor.GetDefaultEvent(this, true); }

        public System.ComponentModel.PropertyDescriptor GetDefaultProperty() { return TypeDescriptor.GetDefaultProperty(this, true); }

        public object GetEditor(Type editorBaseType) { return TypeDescriptor.GetEditor(this, editorBaseType, true); }

        public EventDescriptorCollection GetEvents(Attribute[] attributes) { return TypeDescriptor.GetEvents(this, attributes, true); }

        public EventDescriptorCollection GetEvents() { return TypeDescriptor.GetEvents(this, true); }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            return new PropertyDescriptorCollection(new PropertyDescriptor[] { new ReaderPropertyDescriptor() });
        }

        public PropertyDescriptorCollection GetProperties() { return GetProperties(null); }

        public object GetPropertyOwner(System.ComponentModel.PropertyDescriptor pd) { return this; }

        #endregion

        public class ReaderPropertyDescriptor : PropertyDescriptor
        {
            ReaderEditor editor;
            public ReaderPropertyDescriptor() : base("Export/Import/Edit value", null) { }

            public override object GetEditor(Type editorBaseType)
            {
                if (editorBaseType == typeof(System.Drawing.Design.UITypeEditor))
                {
                    if (editor == null) editor = new ReaderEditor();
                    return editor;
                }
                return base.GetEditor(editorBaseType);
            }

            public override bool CanResetValue(object component) { throw new InvalidOperationException(); }

            public override void ResetValue(object component) { throw new InvalidOperationException(); }

            public override Type PropertyType { get { throw new InvalidOperationException(); } }

            public override object GetValue(object component) { throw new InvalidOperationException(); }

            public override bool IsReadOnly { get { throw new InvalidOperationException(); } }

            public override void SetValue(object component, object value) { throw new InvalidOperationException(); }

            public override Type ComponentType { get { throw new InvalidOperationException(); } }

            public override bool ShouldSerializeValue(object component) { throw new InvalidOperationException(); }
        }

        public class ReaderEditor : UITypeEditor
        {
            ReaderEditorPanel ui;
            public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) { return UITypeEditorEditStyle.DropDown; }
            public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
            {
                IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

                if (ui == null)
                    ui = new ReaderEditorPanel();

                ReaderCTD o = value as ReaderCTD;
                ui.SetField(o.owner, o.field);
                ui.EdSvc = edSvc;
                edSvc.DropDownControl(ui);
                // the ui (a) updates the value and (b) closes the dropdown

                return o.owner[o.field].Value;
            }
        }

        public class ReaderConverter : TypeConverter
        {
            public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            {
                if (typeof(string).IsAssignableFrom(destinationType)) return true;
                return base.CanConvertTo(context, destinationType);
            }

            public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
            {
                if (typeof(string).IsAssignableFrom(destinationType))
                    return "Import/Export/View Hex/Edit...";
                return base.ConvertTo(context, culture, value, destinationType);
            }
        }
    }

    public class ConverterPropertyDescriptor : System.ComponentModel.PropertyDescriptor
    {
        AApiVersionedFields owner;
        string field;
        object component;
        public ConverterPropertyDescriptor(AApiVersionedFields owner, string field, object component, Attribute[] attr)
            : base(field, attr) { this.owner = owner; this.field = field; this.component = component; }

        //public override string Name { get { return field; } }

        public override bool CanResetValue(object component) { throw new InvalidOperationException(); }

        public override void ResetValue(object component) { throw new InvalidOperationException(); }

        public override Type PropertyType { get { throw new InvalidOperationException(); } }

        public override object GetValue(object component) { throw new InvalidOperationException(); }

        public override bool IsReadOnly { get { throw new InvalidOperationException(); } }

        public override void SetValue(object component, object value) { throw new InvalidOperationException(); }

        //public override Type ComponentType { get { throw new InvalidOperationException(); } }
        public override Type ComponentType { get { return component.GetType(); } }

        //public override bool ShouldSerializeValue(object component) { throw new InvalidOperationException(); }
        public override bool ShouldSerializeValue(object component) { return true; }
    }

    public class ArrayOwner : AApiVersionedFields
    {
        AApiVersionedFields owner;
        string field;
        TypedValue tv;
        IList list;
        public ArrayOwner(AApiVersionedFields owner, string field) { this.owner = owner; this.field = field; tv = owner[field]; list = (IList)tv.Value; }

        public Type ElementType { get { return tv.Value.GetType().GetElementType(); } }

        static System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"^\[([\dA-F]+)\].*$");
        public override TypedValue this[string index]
        {
            get
            {
                if (!regex.IsMatch(index))
                    throw new ArgumentOutOfRangeException();
                int i = Convert.ToInt32("0x" + regex.Match(index).Groups[1].Value, 16);

                return new TypedValue(ElementType, ((IList)tv.Value)[i]);
            }
            set
            {
                if (!regex.IsMatch(index))
                    throw new ArgumentOutOfRangeException();
                int i = Convert.ToInt32("0x" + regex.Match(index).Groups[1].Value, 16);

                //list[i] = value.Value; <-- BYPASSES "new" set method in AHandlerList<T>
                //Lists
                PropertyInfo p = list.GetType().GetProperty("Item");
                if (p != null)
                    p.SetValue(list, value.Value, new object[] { i, });
                else
                {
                    //Arrays
                    MethodInfo m = list.GetType().GetMethod("Set");
                    if (m != null)
                        m.Invoke(list, new object[] { i, value.Value });
                    owner[field] = new TypedValue(tv.Type, list);
                }
            }
        }

        public override List<string> ContentFields
        {
            get
            {
                List<string> res = new List<string>();
                string fmt = "[{0:X" + list.Count.ToString("X").Length + "}] {1}";
                for (int i = 0; i < list.Count; i++)
                    res.Add(String.Format(fmt, i, field));
                return res;
            }
        }

        public override int RecommendedApiVersion { get { return 0; } }
    }

    public class AsKVP : AHandlerElement, IEquatable<AsKVP>
    {
        static List<string> contentFields = new List<string>(new string[] { "Key", "Val", });
        DictionaryEntry entry;

        public AsKVP(int apiVersion, EventHandler handler, AsKVP basis) : this(apiVersion, handler, basis.entry.Key, basis.entry.Value) { }
        public AsKVP(int apiVersion, EventHandler handler, DictionaryEntry entry) : this(apiVersion, handler, entry.Key, entry.Value) { }
        public AsKVP(int apiVersion, EventHandler handler, object key, object value) : base(apiVersion, handler) { entry = new DictionaryEntry(key, value); }

        public override List<string> ContentFields { get { return contentFields; } }
        public override int RecommendedApiVersion { get { return 0; } }

        public override TypedValue this[string index]
        {
            get
            {
                switch (contentFields.IndexOf(index))
                {
                    case 0: return new TypedValue(entry.Key.GetType(), entry.Key, "X");
                    case 1: return new TypedValue(entry.Value.GetType(), entry.Value, "X");
                    default: throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (contentFields.IndexOf(index))
                {
                    case 0: entry.Key = value.Value; break;
                    case 1: entry.Value = value.Value; break;
                    default: throw new IndexOutOfRangeException();
                }
            }
        }

        public Type GetType(string index)
        {
            switch (contentFields.IndexOf(index))
            {
                case 0: return entry.Key.GetType();
                case 1: return entry.Value.GetType();
                default: throw new IndexOutOfRangeException();
            }
        }

        public override AHandlerElement Clone(EventHandler handler) { return new AsKVP(requestedApiVersion, handler, this); }

        #region IEquatable<AsKVP> Members

        public bool Equals(AsKVP other) { return this.entry.Key.Equals(other.entry.Key) && this.entry.Value.Equals(other.entry.Value); }

        #endregion
    }

    public class AsKVPList : DependentList<AsKVP>
    {
        DictionaryEntry entry;
        public AsKVPList(DictionaryEntry entry) : base(null) { this.entry = entry; }
        public override void Add() { this.Add(new AsKVP(0, null, entry)); }
        protected override AsKVP CreateElement(Stream s) { throw new NotImplementedException(); }
        protected override void WriteElement(Stream s, AsKVP element) { throw new NotImplementedException(); }
    }
}