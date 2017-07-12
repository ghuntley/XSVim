namespace XSVim
open Mono.Addins
open MonoDevelop.Core
open MonoDevelop.Ide
open MonoDevelop.Components.Commands
open MonoDevelop.Core
open MonoDevelop.Core.Text
open MonoDevelop.Ide
open MonoDevelop.Ide.Commands
open MonoDevelop.Ide.Editor
open MonoDevelop.Ide.Editor.Extension
open System.Threading.Tasks
open Reflection
/// Disable intellisense when in normal mode
module CompletionProviders =
    let completionProviders = ResizeArray<CompletionTextEditorExtension>()
    let addRemove _sender (args:ExtensionNodeEventArgs) =
        let provider = args.ExtensionObject
        match provider with
        | :? CompletionTextEditorExtension as c ->
            match args.Change with
            | ExtensionChange.Add -> completionProviders.Add c
            | ExtensionChange.Remove -> completionProviders.Remove c |> ignore
            | _ -> ()
        | _ -> ()

    AddinManager.AddExtensionNodeHandler ("/MonoDevelop/Ide/TextEditorExtensions", addRemove)

type XSVimCompletion() =
    inherit CompletionTextEditorExtension()
    static let providers = CompletionProviders.completionProviders
    let provider = providers |> Seq.find(fun p -> p.CompletionLanguage = "F#" && p.GetType() <> typeof<XSVimCompletion>)
    member x.GetProvider() =
        provider?Editor <- x.Editor
        provider?DocumentContext <- x.DocumentContext
        provider

    override x.CompletionLanguage = "F#"

    //override x.KeyPress descriptor =
        //x.GetProvider().KeyPress descriptor
    override x.HandleParameterCompletionAsync (context, completionChar, token) =
        x.GetProvider().HandleParameterCompletionAsync (context, completionChar, token)
    override x.HandleCodeCompletionAsync(context, triggerInfo, token) =
        x.GetProvider().HandleCodeCompletionAsync(context, triggerInfo, token)
