using CommunityToolkit.Mvvm.ComponentModel;
using chldr_data.Entities;
using chldr_data.Enums;
using chldr_data.Models;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace chldr_ui.ViewModels
{
    public abstract partial class EntryViewModelBase : ViewModelBase
    {

        #region Properties
        [Parameter]
        public EntryModel Model { get; set; }

        [Parameter]
        public string EntityId { get; set; }
        public string Source { get; set; }
        public string Header { get; set; }
        public string Subheader { get; set; }
        public int Type { get; set; }
        public List<TranslationViewModel> TranslationViewModels { get; } = new();
        #endregion

        #region Actions
        public void ListenToPronunciation() { }
        public void NewTranslation() { }
        public void AddToFavorites() { }
        public void Share() { }
        public void Flag() { }
        #endregion

        #region Contructors
        public EntryViewModelBase() { }
        public EntryViewModelBase(EntryModel entry)
        {
            InitializeViewModel(entry);
        }
        #endregion

        #region EventHandlers

        #endregion

        #region Methods
        protected override void OnInitialized()
        {
            base.OnInitialized();
            if (!string.IsNullOrEmpty(EntityId))
            {
                InitializeViewModel(EntityId);
            }
        }
        protected abstract void InitializeViewModel(string entryId);

        protected virtual void InitializeViewModel(EntryModel entry)
        {
            Source = ParseSource(entry.Source.Name);
            TranslationViewModels.AddRange(entry.Translations.Select(t => new TranslationViewModel(t)));
        }

        private static string ParseSource(string sourceName)
        {
            string sourceTitle = null;
            switch (sourceName)
            {
                case "Maciev":
                    sourceTitle = "Чеченско - русский словарь, А.Г.Мациева";
                    break;
                case "Karasaev":
                    sourceTitle = "Русско - чеченский словарь, Карасаев А.Т., Мациев А.Г.";
                    break;
                case "User":
                    sourceTitle = "Добавлено пользователем";
                    break;
                case "Malaev":
                    sourceTitle = "Чеченско - русский словарь, Д.Б. Малаева";
                    break;
                case "Anatslovar":
                    sourceTitle = "Чеченско-русский, русско-чеченский словарь анатомии человека, Р.У. Берсанова";
                    break;
                case "ikhasakhanov":
                    sourceTitle = "Ислам Хасаханов";
                    break;
            }
            return sourceTitle;
        }
        #endregion

    }
}
