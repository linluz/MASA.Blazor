﻿using BlazorComponent;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;

namespace MASA.Blazor
{
    public partial class MDatePicker : MPicker, IThemeable, IElevatable, IColorable
    {
        private DateTime _tableDate;
        private DateTime _value;

        [Parameter]
        public bool Disabled { get; set; }

        [Parameter]
        public bool Readonly { get; set; }

        [Parameter]
        public string YearIcon { get; set; }

        public int? MinYear => Min?.Year;

        public int? MaxYear => Max?.Year;

        public int? MinMonth => Min?.Month;

        public int? MaxMonth => Max?.Month;

        public DateTime TableDate
        {
            get
            {
                if (_tableDate == default)
                {
                    return Value;
                }

                return _tableDate;
            }
            set
            {
                _tableDate = value;
            }
        }

        public StringNumber TableYear => TableDate.Year;

        public StringNumber TableMonth => TableDate.Month;

        [Parameter]
        public DateTime Value
        {
            get
            {
                if (_value == default)
                {
                    _value = DateTime.Now.Date;
                }

                return _value;
            }
            set
            {
                _value = value;
                _tableDate = value;
            }
        }

        [Parameter]
        public EventCallback<DateTime> ValueChanged { get; set; }

        [Parameter]
        public string NextIcon { get; set; } = "mdi-chevron-right";

        [Parameter]
        public string PrevIcon { get; set; } = "mdi-chevron-left";

        [Parameter]
        public DateTime? Min { get; set; }

        [Parameter]
        public DateTime? Max { get; set; }

        protected override void SetComponentClass()
        {
            base.SetComponentClass();

            CssProvider
                .Merge(cssBuilder =>
                {
                    cssBuilder
                        .Add("m-picker--date");
                });

            AbstractProvider
                .Apply<IPickerTitle, MDatePickerTitle>(props =>
                {
                    props[nameof(MDatePickerTitle.Year)] = (StringNumber)Value.Year;
                    props[nameof(MDatePickerTitle.YearIcon)] = YearIcon;
                    props[nameof(MDatePickerTitle.Date)] = Value.ToString("MM-dd");
                })
                .Apply<IPickerBody, MDatePickerBody>(props =>
                {
                    props[nameof(MDatePickerBody.Color)] = Color ?? "accent";
                    props[nameof(MDatePickerBody.Component)] = this;
                    props[nameof(MDatePickerBody.DateClick)] = EventCallback.Factory.Create<MouseEventArgs>(this, () =>
                    {
                        if (Type == "date")
                        {
                            Type = "month";
                        }
                        else if (Type == "month")
                        {
                            Type = "year";
                        }
                    });
                    props[nameof(MDatePickerBody.OnDaySelected)] = EventCallback.Factory.Create<int>(this, async day =>
                     {
                         Value = new DateTime(TableDate.Year, TableDate.Month, day);
                         if (ValueChanged.HasDelegate)
                         {
                             await ValueChanged.InvokeAsync(Value);
                         }
                     });
                    props[nameof(MDatePickerBody.OnMonthSelected)] = EventCallback.Factory.Create<int>(this, month =>
                    {
                        TableDate = new DateTime(TableDate.Year, month, TableDate.Day);
                        Type = "date";
                    });
                    props[nameof(MDatePickerBody.OnYearSelected)] = EventCallback.Factory.Create<int>(this, year =>
                    {
                        TableDate = new DateTime(year, TableDate.Month, TableDate.Day);
                        Type = "month";
                    });
                });
        }

        public void AddMonths(int month)
        {
            TableDate = TableDate.AddMonths(month);
        }

        public void AddYears(int year)
        {
            TableDate = TableDate.AddYears(year);
        }
    }
}
