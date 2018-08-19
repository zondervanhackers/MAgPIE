using System;

namespace ZondervanLibrary.Harvester.Wpf.Dialogs.Repository.ViewModels
{
    public interface IRepositoryViewModel
    {
        /// <summary>
        /// A name that identifies the type of repository to the user.
        /// </summary>
        String Name { get; }

        /// <summary>
        /// A short blurb that describes the type of repository.
        /// </summary>
        String Description { get; }
    }
}
