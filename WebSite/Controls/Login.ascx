﻿<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Login.ascx.cs" Inherits="Controls_Login"  %>
              
<div data-app-role="page" data-content-framework="bootstrap">
        <div class="navbar navbar-default navbar-static-top" role="navigation">
            <div class="container">
                <div class="navbar-header">
                    <span class="navbar-brand">Standalone Login Page</span>
                </div>
                <div class="navbar-collapse">
                    <div class="navbar-form navbar-right" role="form">
                        <div class="form-group">
                            <input id="login-user-name" type="text" placeholder="Username" class="form-control" autocapitalize="off">
                        </div>
                        <div class="form-group">
                            <input id="login-password" type="password" placeholder="Password" class="form-control">
                        </div>
                        <button id="login-button" class="btn btn-success">Sign in</button>
                    </div>
                </div>
                <!--/.navbar-collapse -->
            </div>
        </div>
        <!-- Main jumbotron for a primary marketing message or call to action -->
        <%--<div class="jumbotron">
            <div class="container">
                <h1><span class="glyphicon glyphicon-globe"></span>&nbsp;Hello, world!</h1>
                <p>
                    This is a template for a custom login screen. It includes a large callout called a jumbotron and three supporting content blocks.
                    Edit the "~/Login.html" page in Visual Studio to create something more unique.
                </p>

                <p><a href="#" class="btn btn-primary btn-lg" role="button">Learn more &raquo;</a></p>
            </div>
        </div>--%>

        <%--<div class="container">

            <div class="row">
                <div class="col-sm-8">
                    <h2>^SignInInstruction^Sign in to access the protected site content.^SignInInstruction^</h2>
                    <p>
                        ^SignInPara1^Two standard user accounts are automatically created when application is initialized
                        if membership option has been selected for this application.^SignInPara1^
                    </p>

                    <p>
                        ^SignInPara2^The administrative account <b>admin</b> is authorized to access all areas of the
                        web site and membership manager. The standard <b>user</b> account is allowed to
                        access all areas of the web site with the exception of membership manager.^SignInPara2^
                    </p>
                </div>
                <div class="col-sm-4">
                    <div class="panel panel-success">
                        <div class="panel-heading">
                            <span>^AdminDesc^Administrative account^AdminDesc^</span>
                        </div>
                        <div class="panel-body">
                            User Name: <b title="User Name">admin</b><br />
                            Password: <b title="Password">admin123%</b><br />
                            <a class="btn btn-link" href="#" id="admin-login">Login as Admin</a><br />
                        </div>
                    </div>

                    <div class="panel panel-default">
                        <div class="panel-heading">
                            <span>^UserDesc^Standard user account^UserDesc^</span>
                        </div>
                        <div class="panel-body">
                            User Name: <b title="User Name">user</b><br />
                            Password: <b title="Password">user123%</b><br />
                            <a class="btn btn-link" href="#" id="user-login">Login as User</a><br />
                        </div>
                    </div>
                </div>
            </div>
            <hr />
            <!-- Example row of columns -->
            <div class="row">
                <div class="col-sm-4">
                    <h2>Heading</h2>
                    <p>Donec id elit non mi porta gravida at eget metus. Fusce dapibus, tellus ac cursus commodo, tortor mauris condimentum nibh, ut fermentum massa justo sit amet risus. Etiam porta sem malesuada magna mollis euismod. Donec sed odio dui. </p>
                    <p><a class="btn btn-default" href="#" role="button">View details &raquo;</a></p>
                </div>
                <div class="col-sm-4">
                    <h2>Heading</h2>
                    <p>Donec id elit non mi porta gravida at eget metus. Fusce dapibus, tellus ac cursus commodo, tortor mauris condimentum nibh, ut fermentum massa justo sit amet risus. Etiam porta sem malesuada magna mollis euismod. Donec sed odio dui. </p>
                    <p><a class="btn btn-default" href="#" role="button">View details &raquo;</a></p>
                </div>
                <div class="col-sm-4">
                    <h2>Heading</h2>
                    <p>Donec sed odio dui. Cras justo odio, dapibus ac facilisis in, egestas eget quam. Vestibulum id ligula porta felis euismod semper. Fusce dapibus, tellus ac cursus commodo, tortor mauris condimentum nibh, ut fermentum massa justo sit amet risus.</p>
                    <p><a class="btn btn-default" href="#" role="button">View details &raquo;</a></p>
                </div>
            </div>

            <hr/>
        <!-- /container -->
        </div>--%>
    </div>


    <script type="text/javascript">
        (function () {
            var resources = Web.MembershipResources.Messages;

            function performLogin(username, password) {

                var userNameElem = $('#login-user-name'),
                    passwordElem = $('#login-password');

                if (!username)
                    username = userNameElem.val();
                if (!password)
                    password = passwordElem.val();

                if (!username)
                    $app.alert(resources.BlankUserName, function () {
                        userNameElem.focus();
                    });
                else if (!password)
                    $app.alert(resources.BlankPassword, function () {
                        passwordElem.focus();
                    });
                else
                    $app.login(username, password, true,
                        function () {
                            setTimeout(function() {
                              $app._navigated = true;
                              var returnUrl = window.location.href.match(/\?ReturnUrl=(.+)$/);
                              window.location.replace(returnUrl && decodeURIComponent(returnUrl[1]) || __baseUrl);
                            });
                        },
                        function () {
                            $app.alert(resources.InvalidUserNameAndPassword, function () {
                                userNameElem.focus();
                            });
                        });
                return false;
            }

            $(document)
                .on('click', '#login-button', function () {
                    performLogin();
                })
                .on('click', '#admin-login', function () {
                    performLogin('admin', 'admin123%');
                })
                .on('click', '#user-login', function () {
                    performLogin('user', 'user123%');
                })
                .on('keydown', 'input', function (event) {
                    if (event.which == 13) {
                        event.preventDefault();
                        performLogin();
                        return false;
                    }
                });
        })();
    </script>

            