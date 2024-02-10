module gol;

constexpr char DEAD = 0;
constexpr char ALIVE = 1;

namespace gol
{
    static char get_next_state(const char* prev, size_t size, size_t i)
    {
        const auto length = size * size;

        const auto ul = i - size - 1;
        const auto up = i - size;
        const auto ur = i - size + 1;
        const auto le = i - 1;
        const auto ri = i + 1;
        const auto dl = i + size - 1;
        const auto dp = i + size;
        const auto dr = i + size + 1;

        size_t living = 0;
        if (ul >= 0 && ul < length && prev[ul] == ALIVE)
            living++;
        if (up >= 0 && up < length && prev[up] == ALIVE)
            living++;
        if (ur >= 0 && ur < length && prev[ur] == ALIVE)
            living++;
        if (le >= 0 && le < length && prev[le] == ALIVE)
            living++;
        if (ri >= 0 && ri < length && prev[ri] == ALIVE)
            living++;
        if (dl >= 0 && dl < length && prev[dl] == ALIVE)
            living++;
        if (dp >= 0 && dp < length && prev[dp] == ALIVE)
            living++;
        if (dr >= 0 && dr < length && prev[dr] == ALIVE)
            living++;

        const auto cell = prev[i];
        if (cell == ALIVE) {
            return living == 2 || living == 3 ? ALIVE : DEAD;
        }
        else {
            return living == 3 ? ALIVE : DEAD;
        }
    }

    void gol::tick(const char* prev, char* next, size_t size)
    {
        // TODO: Make this run in parallel?
        for (size_t y = 0; y < size; ++y) {
            for (size_t x = 0; x < size; ++x) {
                const auto i = y * size + x;
                next[i] = get_next_state(prev, size, i);
            }
        }
    }
}