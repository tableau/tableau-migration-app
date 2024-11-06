# GUI Module

## Design
The GUI module is implemented with [Avalonia](https://github.com/AvaloniaUI/Avalonia) in a MVVM pattern. The GUI is in charge of presenting interactable elements for the user to provide their authentication, and configuration data for the migration. This data is then processed and passed over to the Core Module as an dependency injected service to perform the migration.

The GUI module has the following folders:
- **Assets** - Assets such as icons and data files used for packaging.x
- **Models** - Structures to hold stateful data for the application.
- **Services** - Defined interface and implementations of services to be used in the module.
- **Views** - The axaml definition of GUI screens, as well as their code-behind interactions logic.
- **ViewModels** - The data binding logic associating the model datas with the visible views.

## Entrypoint
Entrypoint of the module is defined in `Program.cs` found in the root directory of the GUI Module project. This entrypoint loads up the Avalonia services to present the GUI found in `App.axaml`.

Once loaded, the Avalonia services are configured in the code-behind file `App.axaml.cs`.

## Services
Currently we have the following defined service interfaces:

- **Window Provider** - Utility service to retrieve an Avalonia application's main window instance.
- **File Picker** - Service open up a file picker and select files.
- **CSV Parser** - Service to parse CSV files provided for username mappings.

## Views
The top level view used to hold all visible elements is located in the [MainWindow](/api/Tableau.Migration.App.GUI.Views.MainWindow.html) view.

## View Models
All ViewModels are named based on their appropriate View name. i.e. the [MainWindow](/api/Tableau.Migration.App.GUI.Views.MainWindow.html) view has an associatedv [MainWindowViewModel](api/Tableau.Migration.App.GUI.ViewModels.MainWindowViewModel.html).

There exist 2 abstract classes defined in the ViewModel folder:
- [**ViewModelBase**](/api/Tableau.Migration.App.GUI.ViewModels.ViewModelBase.html) - The base class of a ViewModel.
- [**ValidateableViewModelBase**](/api/Tableau.Migration.App.GUI.ViewModels.ValidatableViewModelBase.html) - The base class of a ViewModel that will require some form of validation performed on the data that it holds.
