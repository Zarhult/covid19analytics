#include "quitbutton.hpp"
#include <QApplication>
#include <QWidget>

int main(int argc, char *argv[]) {
    QApplication app(argc, argv);

    QuitButton window;

    window.resize(400, 300);
    window.setWindowTitle("Test");
    window.show();

    return app.exec();
}
