import gol;

#define DLLEXPORT __declspec(dllexport)

extern "C" {
    DLLEXPORT void Tick(const char* prev, char* next, size_t size) {
        gol::tick(prev, next, size);
    }
}