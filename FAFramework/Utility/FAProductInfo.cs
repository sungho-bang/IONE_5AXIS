using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FALibrary;

namespace FAFramework.Utility
{
    public enum EProductStatus
    {
        Normal, Fail, QC
    }

    [Serializable]
    public class FAProductInfo : FALibrary.FAObject
    {
        [FAAttribute("")]
        public string UniqueID { get; set; }

        [FAAttribute("")]
        public DateTime InputTime { get; set; }
        
        [FAAttribute("")]
        public bool IsLast { get; set; }
        
        [FAAttribute("")]
        public bool ExistProduct { get; set; }

        [FAAttribute("")]
        public EProductStatus ProductStatus { get; set; } = EProductStatus.Normal;
        
        [FAAttribute("")]
        public string Information { get; set; }


        [FAAttribute("")]
        public string SamsungBarcode { get; set; }

        [FAAttribute("")]
        public string SpecialBarcode { get; set; }

        [FAAttribute("")]
        public int BoxCount { get; set; }

        [FAAttribute("")]
        public bool IsEmptyBox { get; set; }

        /// <summary>
        /// 참조 복사되는 데이타.
        /// 모듈간 데이터 공유를 위해서 정의됨.
        /// </summary>
        public FAObject RefData { get; set; }        

        public virtual void CopyTo(FAProductInfo obj)
        {
            obj.UniqueID = this.UniqueID;
            obj.InputTime = this.InputTime;
            obj.IsLast = this.IsLast;
            obj.ExistProduct = ExistProduct;
            obj.ProductStatus = this.ProductStatus;
            obj.Information = this.Information;
            obj.SamsungBarcode = this.SamsungBarcode;
            obj.SpecialBarcode = this.SpecialBarcode;
            obj.IsEmptyBox = this.IsEmptyBox;
            obj.RefData = this.RefData;
            obj.BoxCount = this.BoxCount;
        }

        public virtual void Clear()
        {
            UniqueID = string.Empty;
            InputTime = DateTime.MinValue;
            IsLast = false;
            ExistProduct = false;
            ProductStatus = EProductStatus.Fail;
            Information = string.Empty;
            SamsungBarcode = string.Empty;
            SpecialBarcode = string.Empty;
            IsEmptyBox = false;
            BoxCount = 0;
        }

        public FAProductInfo Clone()
        {
            var prodInfo = new FAProductInfo();
            this.CopyTo(prodInfo);
            return prodInfo;
        }
    }
}
