using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;

namespace VioGIS
{
    class MapAnalysis
    {
        public bool QueryIntersect(string srcLayerName, string tgtLayerName, IMap imap, esriSpatialRelationEnum spatialRel)
        {
            DataOperator pDo = new DataOperator(imap, null);
            //定义并根据图层名称获取图层对象
            IFeatureLayer iSrcLayer = new FeatureLayerClass();
            iSrcLayer = (IFeatureLayer)pDo.GetLayerbyName(srcLayerName);
            IFeatureLayer iTgtLayer = (IFeatureLayer)pDo.GetLayerbyName(tgtLayerName);
            //通过查询过滤获取continent层中亚洲几何
            IGeometry pGt;
            IFeature pF;
            IFeatureCursor pFcs;
            IFeatureClass pFcl;
            IQueryFilter pQf = new QueryFilter();
            pQf.WhereClause = "CONTINENT='Asia'";//设置查询条件
            pFcs = iTgtLayer.FeatureClass.Search(pQf, false);
            pF = pFcs.NextFeature();
            pGt = pF.Shape;
            try
            {
                pFcl = iSrcLayer.FeatureClass;
            }
            catch
            { 
            }
            ISpatialFilter pSf = new SpatialFilter();
            pSf.Geometry = pGt;
            pSf.WhereClause = "POP_RANK=5";//人口等级低于5的城市
            pSf.SpatialRel = (ESRI.ArcGIS.Geodatabase.esriSpatialRelEnum)spatialRel;
            //定义要素选择对象，以要素搜索图层进行实例化
            IFeatureSelection pFs = (IFeatureSelection)iSrcLayer;
            //以空间过滤器对要素进行选择，并建立新选择集
            try
            {
                pFs.SelectFeatures(pSf, esriSelectionResultEnum.esriSelectionResultNew, false);
            }
            catch
            { 
            }
            return true;
        }

        public bool Buffer(string layerName, string sWhere, int iSize, IMap imap)
        {
            IFeatureClass pFcl;
            IFeature pF;
            IGeometry pGt;
            DataOperator pDo = new DataOperator(imap, null);
            IFeatureLayer pFl = (IFeatureLayer)pDo.GetLayerbyName(layerName);
            pFcl = pFl.FeatureClass;
            IQueryFilter pQf = new QueryFilterClass();
            pQf.WhereClause = sWhere;
            IFeatureCursor pFc;
            pFc = (IFeatureCursor)pFcl.Search(pQf, false);
            int count = pFcl.FeatureCount(pQf);
            pF = pFc.NextFeature();
            pGt = pF.Shape;
            ITopologicalOperator pTo = (ITopologicalOperator)pGt;
            IGeometry pGtBuffer = pTo.Buffer(iSize);
            ISpatialFilter pSf = new SpatialFilter();
            pSf.Geometry = pGtBuffer;
            pSf.SpatialRel = esriSpatialRelEnum.esriSpatialRelIndexIntersects;
            IFeatureSelection pFs = (IFeatureSelection)pFl;
            pFs.SelectFeatures(pSf, esriSelectionResultEnum.esriSelectionResultNew, false);
            return true;
        }

        public string Statistic(string layerName, string fieldName, IMap imap)
        {
            DataOperator pDo = new DataOperator(imap,null);
            IFeatureLayer pFl = (IFeatureLayer)pDo.GetLayerbyName(layerName);
            IFeatureClass pFcl = pFl.FeatureClass;
            IDataStatistics pDst = new DataStatistics();
            IFeatureCursor pFc;
            pFc = pFcl.Search(null,false);
            ICursor pCs = (ICursor)pFc;
            pDst.Cursor = pCs;
            pDst.Field = fieldName;
            IStatisticsResults StartReasult;
            StartReasult = pDst.Statistics;
            double dMax, dMin, dMean;
            dMax = StartReasult.Maximum;
            dMin = StartReasult.Minimum;
            dMean = StartReasult.Mean;
            string sReasult;
            sReasult = "最大面积为"+dMax.ToString()+";最小面积为"+dMin.ToString()+";平均面积为"+dMean.ToString();
            return sReasult;
        }
    }
}
