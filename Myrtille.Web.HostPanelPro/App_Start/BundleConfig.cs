/*
    Myrtille: A native HTML4/5 Remote Desktop Protocol client.

    Copyright(c) 2014-2021 Cedric Coste

    Licensed under the Apache License, Version 2.0 (the "License");
    you may not use this file except in compliance with the License.
    You may obtain a copy of the License at

        http://www.apache.org/licenses/LICENSE-2.0

    Unless required by applicable law or agreed to in writing, software
    distributed under the License is distributed on an "AS IS" BASIS,
    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
    See the License for the specific language governing permissions and
    limitations under the License.
*/

using System.Web.Optimization;

namespace Myrtille.Web
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            // scripts

            // compute the scripts hashes to prevent them from being cached by the browser in case of content changes

            // don't minify, to allow debugging the scripts even in release
            // the objective is to always have the latest scripts code when hitting F5, not to optimize the bandwidth at the expense of readability
            // if you want to minify anyway, replace "Bundle" by "ScriptBundle" below
            // also, the virtual paths for the bundles must be different from those of the included files (then use the bundles paths into default.aspx)

            // it's also possible to concatenate all scripts into a single bundle (that could even be convenient),
            // but then if one script change, all scripts are reloaded/hashed/minified (the bundle is regenerated)

            // more info: https://docs.microsoft.com/en-us/aspnet/mvc/overview/performance/bundling-and-minification
            // https://stackoverflow.com/questions/29517467/force-browser-to-refresh-javascript-code-while-developing-an-mvc-view/29519141#29519141

            bundles.Add(new Bundle("~/rdp/js/tools/common.js").Include("~/rdp/js/tools/common.js"));
            bundles.Add(new Bundle("~/rdp/js/tools/convert.js").Include("~/rdp/js/tools/convert.js"));
            bundles.Add(new Bundle("~/rdp/js/myrtille.js").Include("~/rdp/js/myrtille.js"));
            bundles.Add(new Bundle("~/rdp/js/config.js").Include("~/rdp/js/config.js"));
            bundles.Add(new Bundle("~/rdp/js/dialog.js").Include("~/rdp/js/dialog.js"));
            bundles.Add(new Bundle("~/rdp/js/display.js").Include("~/rdp/js/display.js"));
            bundles.Add(new Bundle("~/rdp/js/display/canvas.js").Include("~/rdp/js/display/canvas.js"));
            bundles.Add(new Bundle("~/rdp/js/display/divs.js").Include("~/rdp/js/display/divs.js"));
            bundles.Add(new Bundle("~/rdp/js/display/terminaldiv.js").Include("~/rdp/js/display/terminaldiv.js"));
            bundles.Add(new Bundle("~/rdp/js/network.js").Include("~/rdp/js/network.js"));
            bundles.Add(new Bundle("~/rdp/js/network/buffer.js").Include("~/rdp/js/network/buffer.js"));
            bundles.Add(new Bundle("~/rdp/js/network/eventsource.js").Include("~/rdp/js/network/eventsource.js"));
            bundles.Add(new Bundle("~/rdp/js/network/longpolling.js").Include("~/rdp/js/network/longpolling.js"));
            bundles.Add(new Bundle("~/rdp/js/network/websocket.js").Include("~/rdp/js/network/websocket.js"));
            bundles.Add(new Bundle("~/rdp/js/network/xmlhttp.js").Include("~/rdp/js/network/xmlhttp.js"));
            bundles.Add(new Bundle("~/rdp/js/user.js").Include("~/rdp/js/user.js"));
            bundles.Add(new Bundle("~/rdp/js/user/keyboard.js").Include("~/rdp/js/user/keyboard.js"));
            bundles.Add(new Bundle("~/rdp/js/user/mouse.js").Include("~/rdp/js/user/mouse.js"));
            bundles.Add(new Bundle("~/rdp/js/user/touchscreen.js").Include("~/rdp/js/user/touchscreen.js"));
            bundles.Add(new Bundle("~/rdp/js/xterm/xterm.js").Include("~/rdp/js/xterm/xterm.js"));
            bundles.Add(new Bundle("~/rdp/js/xterm/addons/fit/fit.js").Include("~/rdp/js/xterm/addons/fit/fit.js"));
            bundles.Add(new Bundle("~/rdp/js/audio/audiowebsocket.js").Include("~/rdp/js/audio/audiowebsocket.js"));

            // nodejs modules shouldn't be modified directly, but compute hashes anyway just in case...
            bundles.Add(new Bundle("~/rdp/node_modules/interactrdp/js/dist/interact.js").Include("~/rdp/node_modules/interactrdp/js/dist/interact.js"));
            bundles.Add(new Bundle("~/rdp/node_modules/simple-keyboard/build/index.js").Include("~/rdp/node_modules/simple-keyboard/build/index.js"));
            bundles.Add(new Bundle("~/rdp/node_modules/simple-keyboard-layouts/build/index.js").Include("~/rdp/node_modules/simple-keyboard-layouts/build/index.js"));

            // styles

            // same comments as above; replace "Bundle" by "StyleBundle" for minification

            bundles.Add(new Bundle("~/rdp/css/Default.css").Include("~/rdp/css/Default.css"));
            bundles.Add(new Bundle("~/rdp/css/xterm.css").Include("~/rdp/css/xterm.css"));

            // nodejs modules, same remark as above...
            bundles.Add(new Bundle("~/rdp/node_modules/simple-keyboard/build/rdp/css/index.css").Include("~/rdp/node_modules/simple-keyboard/build/rdp/css/index.css"));
        }
    }
}