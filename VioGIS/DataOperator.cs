using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesFile;

namespace VioGIS
{
    public class DataOperator
    {
        public IMap m_map;
        public ILayer pSl;

        public DataOperator(IMap map, ILayer SelectedLayer_TOC)
        {
            m_map = map;
            pSl = SelectedLayer_TOC;
        }

        public ILayer GetLayerbyName(string sLayerName)
        {
            if (sLayerName == "" || m_map == null)
            {
                return null;
            }
            for (int i = 0; i < m_map.LayerCount; i++)
            {
                if (m_map.get_Layer(i).Name == sLayerName)
                {
                    return m_map.get_Layer(i);
                }
            }
            return null;
        }

        
        public IFeatureClass CreateShapefile(
            string sParentDirectory,
            string sWorkspaceName,
            string sFileName)
        {
            if (System.IO.Directory.Exists(sParentDirectory + sWorkspaceName))
            {
                System.IO.Directory.Delete(sParentDirectory + sWorkspaceName, true);
            }

            IWorkspaceFactory pWf = new ShapefileWorkspaceFactoryClass();
            IWorkspaceName pWn = pWf.Create(sParentDirectory, sWorkspaceName, null, 0);
            ESRI.ArcGIS.esriSystem.IName name = pWn as ESRI.ArcGIS.esriSystem.IName;

            IWorkspace pW = (IWorkspace)name.Open();
            IFeatureWorkspace pFw = pW as IFeatureWorkspace;

            IFields pFs = new FieldsClass();
            IFieldsEdit pFse = pFs as IFieldsEdit;

            IFieldEdit pFe = new FieldClass();
            pFe.Name_2 = "OID";
            pFe.AliasName_2 = "序号";
            pFe.Type_2 = esriFieldType.esriFieldTypeOID;
            pFse.AddField((IField)pFe);//IFieldEdit.AddField是AO隐藏属性

            pFe = new FieldClass();
            pFe.Name_2 = "Name";
            pFe.AliasName_2 = "名称";
            pFe.Type_2 = esriFieldType.esriFieldTypeString;
            pFse.AddField((IField)pFe);

            IGeometryDefEdit pGde = new GeometryDefClass();
            ISpatialReference pSr = m_map.SpatialReference;
            pGde.SpatialReference_2 = pSr;
            pGde.GeometryType_2 = esriGeometryType.esriGeometryPoint;

            pFe = new FieldClass();
            string sShapeFieldName = "Shape";
            pFe.Name_2 = sShapeFieldName;
            pFe.AliasName_2 = "形状";
            pFe.Type_2 = esriFieldType.esriFieldTypeGeometry;
            pFe.GeometryDef_2 = pGde;
            pFse.AddField((IField)pFe);

            IFeatureClass pFc = pFw.CreateFeatureClass(sFileName, pFs, null, null, esriFeatureType.esriFTSimple, "Shape", "");
            if (pFc == null)
            {
                return null;
            }
            return pFc;
        }

        public bool AddFeatureClassToMap(
            IFeatureClass featureclass,
            string sLayerName)
        {
            if (featureclass == null || sLayerName == "" || m_map == null)
            {
                return false;
            }
            IFeatureLayer pFl = new FeatureLayerClass();
            pFl.FeatureClass = featureclass;
            pFl.Name = sLayerName;

            ILayer pL = pFl as ILayer;
            if (pL == null)
            {
                return false;
            }

            m_map.AddLayer(pL);
            IActiveView pAv = m_map as IActiveView;
            if (pAv == null)
            {
                return false;
            }

            pAv.Refresh();
            return true;
        }

        public bool AddFeatureToLayer(
            string sLayerName,
            string sFeatureName,
            IPoint point)
        {
            if (sLayerName == "" || sFeatureName == "" || point == null || m_map == null)
            {
                return false;
            }

            ILayer pL = null;
            for (int i = 0; i < m_map.LayerCount; i++)
            {
                pL = m_map.get_Layer(i);
                if (pL.Name == sLayerName)
                {
                    break;
                }

                pL = null;
            }
            if (pL == null)
            {
                return false;
            }

            IFeatureLayer pFl = pL as IFeatureLayer;
            IFeatureClass pFc = pFl.FeatureClass;

            IFeature pF = pFc.CreateFeature();
            if (pF == null)
            {
                return false;
            }

            pF.Shape = point;
            int index = pF.Fields.FindField("Name");
            pF.set_Value(index, sFeatureName);
            pF.Store();
            if (pF == null)
            {
                return false;
            }

            IActiveView pAv = m_map as IActiveView;
            
            if (pAv == null)
            {
                return false;
            }

            pAv.Refresh();
            return true;
        }
    }
}
