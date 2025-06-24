angular.module("todoApp", []).controller("TodoController", [
  "$scope",
  "$http",
  "$window",
  function ($scope, $http, $window) {
    const apiBaseUrl = "https://localhost:7200/api/todo";

    const token = localStorage.getItem("token");
    if (!token) {
      $window.location.href = "login.html"; // Not logged in, redirect
      return;
    }

    // Auth header for all requests
    const authHeader = {
      headers: {
        Authorization: "Bearer " + token,
      },
    };

    $scope.todos = [];
    $scope.newTodo = "";

    //  Load todos on page load
    $http
      .get(apiBaseUrl, authHeader)
      .then((res) => {
        $scope.todos = res.data;
        const connection = new signalR.HubConnectionBuilder()
          .withUrl("https://localhost:7200/todoHub", {
            accessTokenFactory: () => localStorage.getItem("token"),
          })
          .build();

        connection
          .start()
          .then(() => console.log("SignalR connected."))
          .catch((err) => console.error("SignalR connection failed:", err));

        connection.on("TodoUpdated", function (updatedTodo) {
          const index = $scope.todos.findIndex((t) => t.id === updatedTodo.id);
          if (index !== -1) {
            $scope.todos[index] = {
              ...updatedTodo,
              justUpdated: true, // trigger badge
            };

            // Auto-hide badge after 3 seconds
            setTimeout(() => {
              $scope.todos[index].justUpdated = false;
              $scope.$apply(); // trigger UI update
            }, 3000);

            $scope.$apply();
          }
        });
        // When a new todo is added by another user
        connection.on("TodoAdded", function (newTodo) {
          const exists = $scope.todos.some((t) => t.id === newTodo.id);
          if (!exists) {
            $scope.todos.push(newTodo);
            $scope.$apply();
          }
        });

        // When a todo is deleted by another user
        connection.on("TodoDeleted", function (deletedId) {
          const index = $scope.todos.findIndex((t) => t.id === deletedId);
          if (index !== -1) {
            $scope.todos.splice(index, 1);
            $scope.$apply();
          }
        });
      })
      .catch((err) => {
        console.error("Error loading todos:", err);
        if (err.status === 401) $window.location.href = "login.html";
      });

    //  Add new todo
    $scope.addTodo = function () {
      const todo = {
        task: $scope.newTodo,
        isDone: false,
        createdDate: new Date().toISOString(),
      };

      $http
        .post(apiBaseUrl, todo, authHeader)
        .then((res) => {
          $scope.todos.push(res.data);
          $scope.newTodo = "";
        })
        .catch((err) => {
          console.error("Add error:", err);
          $scope.error = err.data?.message || "Failed to add todo.";
        });
    };

    //  Remove todo by ID
    $scope.removeTodo = function (index) {
      const todo = $scope.todos[index];

      $http
        .delete(`${apiBaseUrl}/${todo.id}`, authHeader)
        .then(() => {
          $scope.todos.splice(index, 1);
        })
        .catch((err) => console.error("Delete error:", err));
    };

    // Watch for toggle updates and auto-save
    $scope.$watch(
      "todos",
      function (newTodos, oldTodos) {
        newTodos.forEach((todo, i) => {
          if (todo !== oldTodos[i]) {
            // PUT on each change
            $http
              .put(`${apiBaseUrl}/${todo.id}`, todo, authHeader)
              .catch((err) => console.error("Update error:", err));
          }
        });
      },
      true
    );

    $scope.getCompletedCount = function () {
      return $scope.todos.filter((todo) => todo.isDone).length;
    };

    $scope.getRemainingCount = function () {
      return $scope.todos.filter((todo) => !todo.isDone).length;
    };

    $scope.toggleAll = function () {
      const allDone = $scope.todos.every((todo) => todo.isDone);
      $scope.todos.forEach((todo) => (todo.isDone = !allDone));
    };

    $scope.clearCompleted = function () {
      const completedTodos = $scope.todos.filter((todo) => todo.isDone);
      completedTodos.forEach((todo) => {
        $http
          .delete(`${apiBaseUrl}/${todo.id}`, authHeader)
          .catch((err) => console.error("Delete error:", err));
      });

      $scope.todos = $scope.todos.filter((todo) => !todo.isDone);
    };

    $scope.logout = function () {
      localStorage.removeItem("token");
      $window.location.href = "login.html";
    };   
  },
]);
