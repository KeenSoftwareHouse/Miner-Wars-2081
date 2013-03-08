using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWarsMath;
using System.IO;

namespace MinerWars.CommonLIB.AppCode.Import
{
	/// <summary>
	/// material params for export
	/// </summary>
	public class MyMaterialDescriptor
	{
        public string MaterialName { get; private set; }
		public Vector3 m_DiffuseColor = Vector3.One;
		public float m_Glossiness;
        public Vector3 m_SpecularColor = Vector3.Zero;

		public string m_DiffuseTextureName = null;

		/// <summary>
		/// c-tor
		/// </summary>
		/// <param name="materialName"></param>
		public MyMaterialDescriptor(string materialName)
		{
			MaterialName = materialName;
		}


        /// <summary>
        /// c-tor
        /// </summary>
        /// <param name="materialName"></param>
        public MyMaterialDescriptor() {;}

		/// <summary>
		/// Write to binary file
		/// </summary>
		/// <param name="writer"></param>
		/// <returns></returns>
		public bool Write(BinaryWriter writer)
		{
			writer.Write((MaterialName != null) ? MaterialName : "");
			writer.Write((m_DiffuseTextureName != null) ? m_DiffuseTextureName : "");

			writer.Write(m_Glossiness);
			writer.Write(m_DiffuseColor.X);
			writer.Write(m_DiffuseColor.Y);
			writer.Write(m_DiffuseColor.Z);
            writer.Write(m_SpecularColor.X);
            writer.Write(m_SpecularColor.Y);
            writer.Write(m_SpecularColor.Z);

			return true;
		}

		public bool Read(BinaryReader reader)
		{
			MaterialName = reader.ReadString();
			if (String.IsNullOrEmpty(MaterialName))
				MaterialName = null;
			m_DiffuseTextureName = reader.ReadString();
			if (String.IsNullOrEmpty(m_DiffuseTextureName))
				m_DiffuseTextureName = null;

			m_Glossiness = reader.ReadSingle();
			m_DiffuseColor.X = reader.ReadSingle();
			m_DiffuseColor.Y = reader.ReadSingle();
			m_DiffuseColor.Z = reader.ReadSingle();
            m_SpecularColor.X = reader.ReadSingle();
            m_SpecularColor.Y = reader.ReadSingle();
            m_SpecularColor.Z = reader.ReadSingle();

			return true;
		}
	}
}
