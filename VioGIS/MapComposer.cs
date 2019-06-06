using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesFile;


namespace VioGIS
{
    class MapComposer
    {
        public static string GetRendererTypeByLayer(ILayer layer)
        {
            if (layer == null)
            {
                return "图层获取失败";
            }

            IFeatureLayer pFl = layer as IFeatureLayer;
            IGeoFeatureLayer pGfl = layer as IGeoFeatureLayer;
            IFeatureRenderer pFr = pGfl.Renderer;

            if (pFr is ISimpleRenderer)
            {
                return "SimpleRenderer";
            }
            else if (pFr is IUniqueValueRenderer)
            {
                return "UniqueValueRenderer";
            }
            else if (pFr is IDotDensityRenderer)
            {
                return "DotDensityRenderer";
            }
            else if (pFr is IChartRenderer)
            {
                return "ChartRenderer";
            }
            else if (pFr is IProportionalSymbolRenderer)
            {
                return "ProportionalSymbolRenderer";
            }
            else if (pFr is IRepresentationRenderer)
            {
                return "RepresentationRenderer";
            }
            else if (pFr is IClassBreaksRenderer)
            {
                return "ClassBreaksRenderer";
            }
            else if (pFr is IBivariateRenderer)
            {
                return "BivatiateRenderer";
            }
            return "未知或渲染器获取失败";
        }

        public static ISymbol GetSymbolFromLayer(ILayer layer)
        {
            if (layer == null)
            {
                return null;
            }

            IFeatureLayer pFl = layer as IFeatureLayer;
            IFeatureCursor pFc = pFl.Search(null, false);
            IFeature pF = pFc.NextFeature();
            if (pF == null)
            {
                return null;
            }

            IGeoFeatureLayer pGfl = pFl as IGeoFeatureLayer;
            IFeatureRenderer pFr = pGfl.Renderer;
            if (pFr == null)
            {
                return null;
            }

            ISymbol pSb = pFr.get_SymbolByFeature(pF);
            return pSb;
        }

        public static bool RenderSimply(ILayer layer, IColor color)
        {
            if (layer == null || color == null)
            {
                return false;
            }

            ISymbol pSb = GetSymbolFromLayer(layer);
            if (pSb == null)
            {
                return false;
            }

            IFeatureLayer pFl = layer as IFeatureLayer;
            IFeatureClass pFc = pFl.FeatureClass;
            if (pFc == null)
            {
                return false;
            }

            esriGeometryType pGt = pFc.ShapeType;
            switch (pGt)
            {
                case esriGeometryType.esriGeometryPoint:
                    {
                        IMarkerSymbol pMs = pSb as IMarkerSymbol;
                        pMs.Color = color;
                        break;
                    }
                case esriGeometryType.esriGeometryMultipoint:
                    {
                        IMarkerSymbol pMs = pSb as IMarkerSymbol;
                        pMs.Color = color;
                        break;
                    }
                case esriGeometryType.esriGeometryPolyline:
                    {
                        ISimpleLineSymbol pSls = pSb as ISimpleLineSymbol;
                        pSls.Color = color;
                        break;
                    }
                case esriGeometryType.esriGeometryPolygon:
                    {
                        IFillSymbol pFs = pSb as IFillSymbol;
                        pFs.Color = color;
                        break;
                    }
                default:
                    return false;
            }

            ISimpleRenderer pSr = new SimpleRendererClass();
            pSr.Symbol = pSb;
            IFeatureRenderer pFr = pSr as IFeatureRenderer;
            if (pFr == null)
            {
                return false;
            }

            IGeoFeatureLayer pGfl = pFl as IGeoFeatureLayer;
            pGfl.Renderer = pFr;
            return true;
        }
    }
}
